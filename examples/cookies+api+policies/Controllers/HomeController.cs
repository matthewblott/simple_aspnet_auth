using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace simple_aspnet_auth.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
  [Route("~/")]
  public IActionResult Index() => View();

}