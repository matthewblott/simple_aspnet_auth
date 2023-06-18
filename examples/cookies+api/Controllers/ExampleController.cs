using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace simple_aspnet_auth;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;

public class ExampleController : Controller
{
  // All
  [Authorize]
  [HttpGet("~/auth")]
  public string All() => "Only authenticated requests receive this message.";

  // Cookies
  [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = GroupNames.SuperUsers + "," + GroupNames.Admins)]
  [HttpGet("~/superuser")]
  public string CookieSuperUser() => "Only authenticated requests from superusers receive this message.";

  [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = GroupNames.Admins)]
  [HttpGet("~/admin")]
  public string CookieAdmin() => "Only authenticated requests from admins receive this message.";

  // Api
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("~/api/auth")]
  public string ApiAuth() => "Only authenticated token based requests receive this message.";

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = GroupNames.SuperUsers + "," + GroupNames.Admins)]
  [Route("~/api/superuser")]
  public string ApiSuperUser() => "Only authenticated token based requests from superusers receive this message.";

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = GroupNames.Admins)]
  [Route("~/api/admin")]
  public string ApiAdmin() => "Only authenticated token based requests from admins receive this message.";

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("~/api/tokeninfo")]
  public IActionResult TokenInfo()
  {
    var authorization = this.Request.Headers["Authorization"].ToString();
    var tokenstring = authorization.Substring("Bearer ".Length).Trim();
    var handler = new JwtSecurityTokenHandler();
    var token = handler.ReadJwtToken(tokenstring);

    return Ok(new { token });

  }

}