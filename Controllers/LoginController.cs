using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace simple_aspnet_auth
{
  public class LoginController : Controller
  {
    ITokenService tokenService;
    IUserService userService;
    public LoginController(ITokenService tokenService, IUserService userService)
    {
      this.tokenService = tokenService;
      this.userService = userService;
    }

    [HttpPost("~/api/signup")]
    public IActionResult Signup(User userViewModel)
    {
      var user = this.userService.GetByName(userViewModel.Name);

      if (user != null)
        return StatusCode(409);

      this.userService.Add(new User
      {
        Name = userViewModel.Name,
        Password = userViewModel.Password
      });

      return Ok(user);
    }

    [HttpPost("~/api/login")]
    public IActionResult Login(User userViewModel)
    {
      var user = this.userService.GetByName(userViewModel.Name);

      if (user == null || userViewModel.Password != user.Password)
        return BadRequest();

      var claims = new[]
      {
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Role, GroupNames.Admins)
      };

      var token = this.tokenService.GenerateAccessToken(claims);
      var refreshToken = this.tokenService.GenerateRefreshToken();

      user.RefreshToken = refreshToken;

      this.userService.Update(user);

      return new ObjectResult(new
      {
        token = token,
        refreshToken = refreshToken
      });

    }

  }

}