using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace simple_aspnet_auth
{
  public class ExampleController : Controller
  {
    [Authorize]
    [HttpGet("~/auth")]
    public string All() =>  "Only authenticated requests receive this message.";

    [Authorize(Roles = GroupNames.SuperUsers + "," + GroupNames.Admins)]
    [HttpGet("~/superuser")]
    public string CookieSuperUser() => "Only authenticated requests from superusers receive this message.";

    [Authorize(Roles = GroupNames.Admins)]
    [HttpGet("~/admin")]
    public string CookieAdmin() => "Only authenticated requests from admins receive this message.";

  }

}
