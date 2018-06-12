using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace simple_aspnet_auth
{
  public class ExampleController : Controller
  {
    [Route("~/api/test")]
    [Authorize(Roles = GroupNames.SuperUsers + "," + GroupNames.Admins)]
    public IActionResult Test() =>
      Content($"The user: {User.Identity.Name} made an authenticated call at {DateTime.Now.ToString("HH:mm:ss")}", "text/plain");

    [Route("~/api/auth")]
    [Authorize]
    public string ApiAuth() => "Only authenticated token based requests receive this message.";

    [Route("~/api/superuser")]
    [Authorize(Roles = GroupNames.SuperUsers + "," + GroupNames.Admins)]
    public string ApiSuperUser() => "Only authenticated token based requests from superusers receive this message.";

    [Route("~/api/admin")]
    [Authorize(Roles = GroupNames.Admins)]
    public string ApiAdmin() => "Only authenticated token based requests from admins receive this message.";

    [Route("~/api/tokeninfo")]
    [Authorize]
    public IActionResult TokenInfo()
    {
      var authorization = this.Request.Headers["Authorization"].ToString();
      var tokenstring = authorization.Substring("Bearer ".Length).Trim();
      var handler = new JwtSecurityTokenHandler();
      var token = handler.ReadJwtToken(tokenstring);

      return Ok(new { token });

    }

  }

}