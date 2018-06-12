using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace simple_aspnet_auth
{
  public class HomeController : Controller
  {
    [Authorize]
    [Route("~/api/test")]
    public IActionResult Test()
    {
      return Content($"The user: {User.Identity.Name} made an authenticated call at {DateTime.Now.ToString("HH:mm:ss")}", "text/plain");
    }
  }
}