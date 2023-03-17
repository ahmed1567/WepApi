using AutoMapper;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;
using WepApiTest;
using AspNetCoreRateLimit;

var rateLimitRules = new List<RateLimitRule>
{
    new RateLimitRule
    {
        Endpoint="*",
        Limit=1,
        Period ="5s"
    }    
    //new RateLimitRule
    //{
    //    Endpoint="*",
    //    Limit=1,
    //    Period ="1s"
    //}
};


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                                                                     .EnableDetailedErrors()
                                                                     .EnableSensitiveDataLogging()
                                                                     .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthManager, AuthManager>();


//builder.Services.AddIdentityCore<ApiUser>(Op=>Op.User.RequireUniqueEmail=true).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<ApiUser, IdentityRole>(Op => Op.User.RequireUniqueEmail = true)
.AddDefaultTokenProviders()
.AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:IssuerSigningKey"]))
        };
    });




builder.Services.AddControllers(config =>
{
    config.CacheProfiles.Add("120SecondDuration", new CacheProfile { Duration = 120 });
}).AddNewtonsoftJson(op =>
        op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(C =>  C.SwaggerDoc("V1", new OpenApiInfo { Title = "TestWepApi", Version = "V1" }) );

builder.Host.UseSerilog((Ctx, LC) => LC.WriteTo.File(
    path: "D:\\Training tasks\\WepApiTest\\WepApiTest\\logs\\log-.txt",
    outputTemplate: "[{Timestamp:HH:mm:ss} {SourceContext} [{Level}] {Message}{NewLine}{Exception}",
    rollingInterval: RollingInterval.Day,
    restrictedToMinimumLevel: LogEventLevel.Information
    ));

builder.Services.AddCors(Options =>
{
    Options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddAutoMapper(typeof(InitialMapper));

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new HeaderApiVersionReader("api-version");
});



builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint="*",
            Limit=1,
            Period ="5s"
        }    
        //new RateLimitRule
        //{
        //    Endpoint="*",
        //    Limit=1,
        //    Period ="1s"
        //}
    };
});

builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

builder.Services.AddHttpContextAccessor();



builder.Services.AddResponseCaching();
builder.Services.AddHttpCacheHeaders(
       (expirationOpt) =>
       {
           expirationOpt.MaxAge = 120;
           expirationOpt.CacheLocation = CacheLocation.Private;
       },
       (validationOpt) =>
       {
           validationOpt.MustRevalidate = true;
       }
    );


var app = builder.Build();


app.UseExceptionHandler(error =>
{
    error.Run(
        async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            var contextFeatures = context.Features.Get<IExceptionHandlerFeature>();
            if (contextFeatures != null)
            {
                Log.Error($"Something Went Wrong in the {contextFeatures.Error}");
                await context.Response.WriteAsync(new Error
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Internal Server Error , Please Try again Later"
                }.ToString());
            }
        });
});




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(s => s.SwaggerEndpoint("V1/swagger.json", "WepApi"));
}

app.UseResponseCaching();
app.UseHttpCacheHeaders();
app.UseIpRateLimiting();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();

app.Run();

