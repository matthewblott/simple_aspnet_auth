using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace simple_aspnet_auth.demo
{
  [AllowAnonymous]
  public class HomeController : Controller
  {
    [Route("~/")]
    public IActionResult Index() => View();

  }

}
