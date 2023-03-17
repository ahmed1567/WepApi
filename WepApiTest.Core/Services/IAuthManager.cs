using WepApiTest.Data;
namespace WepApiTest.Core;

public interface IAuthManager
{

    Task<bool> ValidateUser(LoginDTO userDTO);

    Task<string> CreateToken(ApiUser user);
}
