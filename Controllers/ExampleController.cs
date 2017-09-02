using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;

namespace simple_aspnet_auth
{
  public class ExampleController : Controller
  {
    // Api and Cookie
    [HttpGet]
    [Route("~/auth")]
    [Authorize]
    public string All()
    {
      return "Only authenticated requests receive this message.";
    }

    // Api
    [Route("~/api/auth")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public string ApiAuth()
    {
      return "Only authenticated token based requests receive this message.";
    }

    [HttpGet]
    [Route("~/api/superuser")]
    [Authorize(Policy = GroupNames.SuperUsers, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public string ApiSuperUser()
    {
      return "Only authenticated token based requests from superusers receive this message.";
    }

    [HttpGet]
    [Route("~/api/admin")]
    [Authorize(Policy = GroupNames.Admins, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public string ApiAdmin()
    {
      return "Only authenticated token based requests from admins receive this message.";
    }

    // Cookie
    [HttpGet]
    [Route("~/cookie/auth")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public string CookieAuth()
    {
      return "Only authenticated cookie based requests from superusers receive this message.";
    }

    [HttpGet]
    [Route("~/cookie/superuser")]
    [Authorize(Policy = GroupNames.SuperUsers, AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public string CookieSuperUser()
    {
      return "Only authenticated cookie based requests from superusers receive this message.";
    }

    [HttpGet]
    [Route("~/cookie/admin")]
    [Authorize(Policy = GroupNames.Admins, AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public string CookieAdmin()
    {
      return "Only authenticated cookie based requests from admins receive this message.";
    }

    // Other examples
    [HttpGet]
    [Route("~/api/tokeninfo")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult TokenInfo()
    {
      var authorization = Request.Headers["Authorization"].ToString();
      var tokenstring = authorization.Substring("Bearer ".Length).Trim();
      var handler = new JwtSecurityTokenHandler();
      var token = handler.ReadJwtToken(tokenstring);

      return Ok(new { token });

    }

  }

}
