using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace simple_aspnet_auth
{
  public class LoginController : Controller
  {
    IUserService userService;
    JwtSettings jwtSettings;

    public LoginController(IUserService userService, IOptions<JwtSettings> settings)
    {
      this.userService = userService;
      this.jwtSettings = settings.Value;
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("~/login")]
    public IActionResult Index()
    {
      return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("~/login")]
    public async Task<IActionResult> Login(User userViewModel)
    {
      var returnUrl = "/";

      var user = this.userService.GetByName(userViewModel.Name);

      if (user.Password != userViewModel.Password)
      {
        return LocalRedirect(returnUrl);
      }

      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.Name, user.Name),
      };

      foreach (var group in user.Groups)
      {
        claims.Add(new Claim(group.Name, group.Id.ToString()));
      }

      var props = new AuthenticationProperties
      {
        IsPersistent = true,
        ExpiresUtc = DateTime.UtcNow.AddMinutes(5),
      };

      var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
      var principal = new ClaimsPrincipal(identity);

      await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

      return LocalRedirect(returnUrl);

    }

    [HttpPost]
    [Route("~/logout")]
    public IActionResult Logout()
    {
      this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

      var returnUrl = "/";

      return LocalRedirect(returnUrl);

    }

    // Api
    [AllowAnonymous]
    [HttpPost]
    [Route("~/api/login")]
    public IActionResult ApiLogin(User userViewModel)
    {
      var user = this.userService.GetByName(userViewModel.Name);

      if (user.Password != userViewModel.Password)
      {
        return BadRequest("Could not create token");
      }

      var claims = new List<Claim>
      {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, user.Name),
      };

      foreach (var g in user.Groups)
      {
        claims.Add(new Claim(g.Name, g.Id.ToString()));
      }

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
        jwtSettings.Issuer,
        jwtSettings.Audience,
        claims,
        expires: DateTime.Now.AddMinutes(5),
        signingCredentials: creds);

      var handler = new JwtSecurityTokenHandler();

      return Ok(new { token = handler.WriteToken(token) });

    }

    [HttpPost]
    [Route("~/api/logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult ApiLogout()
    {
      // todo
      return Ok();
    }

  }

}
