using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace simple_aspnet_auth
{
  public class ExampleController : Controller
  {
    [Route("~/api/test")]
    [Authorize(Roles=GroupNames.SuperUsers + "," + GroupNames.Admins)]
    public IActionResult Test() =>
      Content($"The user: {User.Identity.Name} made an authenticated call at {DateTime.Now.ToString("HH:mm:ss")}", "text/plain");

  }

}