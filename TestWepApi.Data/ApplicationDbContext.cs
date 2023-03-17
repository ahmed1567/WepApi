using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WepApiTest.Data;

public class ApplicationDbContext:IdentityDbContext<ApiUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Country>().HasData(
            new Country()
            {
                Id = 1,
                Name = "jamaica"
                ,
                ShortName = "jam",
            },
            new Country()
            {
                Id = 2,
                Name = "Bahams"
                ,
                ShortName = "Bs",
            }
            );        
        
        modelBuilder.Entity<Hotel>().HasData(
            new Hotel()
            {
                Id = 1,
                Name = "jamaica",
                Address="fdddf",
                CountryId=1
               
            }, 
            new Hotel()
            {
                Id = 2,
                Name = "Bahams",
                Address="dfsdds",
                CountryId=1
            }
            );

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole() { 
                Name = "User", 
                NormalizedName = "USER" 
            },            
            
            new IdentityRole() { 
                Name = "Administrator", 
                NormalizedName = "ADMINISTRATOR" 
            }
            );

        modelBuilder.Entity<ApiUser>().ToTable("Users");
    }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
}


