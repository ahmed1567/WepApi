using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
 using WepApiTest.Data;
using System.Security.Claims;
using System.Text;

namespace WepApiTest.Core;

public class AuthManager : IAuthManager
{
    private readonly UserManager<ApiUser> _userManager;

    public AuthManager(UserManager<ApiUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task<bool> ValidateUser(LoginDTO userDTO)
    {
        var user = await _userManager.FindByNameAsync(userDTO.Email);
        return (user != null && await _userManager.CheckPasswordAsync(user,userDTO.Password));
    }

    public async Task<string> CreateToken(ApiUser user)
    {
        var _user = await _userManager.FindByNameAsync(user.Email);
        var signingCredentials = GetSigningCredentials();
        var Claims = await GetClaims(_user);
        var token = GenerateTokenOptions(signingCredentials, Claims);

        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var tokeOptions = new JwtSecurityToken(
                    issuer: "TestWepApi",
                    audience: "TestWepApi2",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: signingCredentials
                 );

        return tokeOptions;
    }

    private async Task<List<Claim>> GetClaims(ApiUser user)
    {
        var claims = new List<Claim>() { new Claim(ClaimTypes.Name,user.UserName)}; 
        var roles=await _userManager.GetRolesAsync(user);
        
        foreach(var role in roles) 
        { 
           claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;

    }

    private SigningCredentials GetSigningCredentials()
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("dkldklfjjdfkdsjjkdsfddfdf"));

        return new SigningCredentials(secretKey,SecurityAlgorithms.HmacSha256);

    }


}
