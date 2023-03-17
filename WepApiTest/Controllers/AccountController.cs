using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WepApiTest;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IMapper _mapper;
    private readonly UserManager<ApiUser> _userManager;
    private readonly SignInManager<ApiUser> _signInManager;
    private readonly IAuthManager _authManager;

    public AccountController(
        ILogger<AccountController> logger,
        IMapper mapper,
        UserManager<ApiUser> userManager,
        SignInManager<ApiUser> signInManager,
        IAuthManager authManager)
    {
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _authManager = authManager;
    }

    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
    {
        _logger.LogInformation($"Registration Attempt for {userDTO.Email}");
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = _mapper.Map<ApiUser>(userDTO);
        user.UserName = userDTO.Email;
        var result = await _userManager.CreateAsync(user, userDTO.Password);

        if (!result.Succeeded)
        {

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);

            }
            return BadRequest(ModelState);
        }

        await _userManager.AddToRolesAsync(user, userDTO.Roles);

        return Ok("Registration Completed");

    }

    [HttpPost]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
    {
        _logger.LogInformation($"Login Attempt for {loginDTO.Email}");
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }



        if (await _authManager.ValidateUser(loginDTO) == false)
        {
            return Unauthorized();
        }

        var userDTO = _mapper.Map<ApiUser>(loginDTO);
        var Token = _authManager.CreateToken(userDTO);
        return Accepted(new { Token = Token.Result });


    }

}
