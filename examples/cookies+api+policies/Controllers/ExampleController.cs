using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace simple_aspnet_auth;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;

public class ExampleController : Controller
{
  // Api and Cookie
  [Authorize]
  [HttpGet("~/auth")]
  public string All() => "Only authenticated requests receive this message.";

  // Api
  [Route("~/api/auth")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public string ApiAuth() => "Only authenticated token based requests receive this message.";

  [Authorize(Policy = GroupNames.SuperUsers, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet("~/api/superuser")]
  public string ApiSuperUser() => "Only authenticated token based requests from superusers receive this message.";

  [Authorize(Policy = GroupNames.Admins, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet("~/api/admin")]
  public string ApiAdmin() => "Only authenticated token based requests from admins receive this message.";

  // Cookie
  [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
  [HttpGet("~/cookie/auth")]
  public string CookieAuth() => "Only authenticated cookie based requests from superusers receive this message.";

  [Authorize(Policy = GroupNames.SuperUsers, AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
  [HttpGet("~/cookie/superuser")]
  public string CookieSuperUser() => "Only authenticated cookie based requests from superusers receive this message.";

  [Authorize(Policy = GroupNames.Admins, AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
  [HttpGet("~/cookie/admin")]
  public string CookieAdmin() => "Only authenticated cookie based requests from admins receive this message.";

  // Other examples
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet("~/api/tokeninfo")]
  public IActionResult TokenInfo()
  {
    var authorization = this.Request.Headers["Authorization"].ToString();
    var tokenstring = authorization.Substring("Bearer ".Length).Trim();
    var handler = new JwtSecurityTokenHandler();
    var token = handler.ReadJwtToken(tokenstring);

    return Ok(new { token });

  }

}