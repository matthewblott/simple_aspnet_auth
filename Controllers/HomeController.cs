using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace simple_aspnet_auth.Controllers
{
  [AllowAnonymous]
  public class HomeController : Controller
  {
		[Route("~/")]
		public IActionResult Index()
    {
      return View();
    }

		[Route("~/about")]
		public IActionResult About()
    {
      ViewData["Message"] = "Your application description page.";

      return View();
    }

		[Route("~/contact")]
		public IActionResult Contact()
    {
      ViewData["Message"] = "Your contact page.";

      return View();
    }

    [Route("~/faq")]
		public IActionResult Faq()
		{
			ViewData["Message"] = "Your frequently asked questions page.";

      var request = this.Request;
      var host = request.Host;
      var address = host.Value;
			var scheme = request.Scheme;
      var newAddress = $"{scheme}://{address}/api";

			return View();

		}

  }

}