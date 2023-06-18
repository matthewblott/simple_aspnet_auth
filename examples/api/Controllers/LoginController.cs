using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace simple_aspnet_auth;

public class LoginController : Controller
{
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;
  
  public LoginController(ITokenService tokenService, IUserService userService)
  {
    _tokenService = tokenService;
    _userService = userService;
  }

  [HttpPost("~/api/signup")]
  public IActionResult Signup(User userViewModel)
  {
    var user = _userService.GetByName(userViewModel.Name);

    if (user != null)
      return StatusCode(409);

    _userService.Add(new User
    {
      Name = userViewModel.Name,
      Password = userViewModel.Password
    });

    return Ok(user);
  }

  [HttpPost("~/api/login")]
  public IActionResult Login(User userViewModel)
  {
    var user = _userService.GetByName(userViewModel.Name);

    if (user == null || userViewModel.Password != user.Password)
      return BadRequest();

    var claims = new[]
    {
      new Claim(ClaimTypes.Name, user.Name),
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new Claim(ClaimTypes.Role, GroupNames.Admins)
    };

    var token = _tokenService.GenerateAccessToken(claims);
    var refreshToken = _tokenService.GenerateRefreshToken();

    user.RefreshToken = refreshToken;

    _userService.Update(user);

    return new ObjectResult(new
    {
      token, refreshToken
    });

  }

}