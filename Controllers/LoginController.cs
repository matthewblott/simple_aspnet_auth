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

    [HttpPost]
    [Route("~/api/signup")]
    public IActionResult Signup(string name, string password)
    {
      var user = this.userService.GetByName(name);

      if (user != null)
        return StatusCode(409);

      this.userService.Add(new User
      {
        Name = name,
        Password = password
      });

      return Ok(user);
    }

    [HttpPost]
    [Route("~/api/login")]
    public IActionResult Login(string name, string password)
    {
      var user = this.userService.GetByName(name);

      if (user == null || password != user.Password)
        return BadRequest();

      var claims = new[]
      {
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
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