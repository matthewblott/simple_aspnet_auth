using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

// See
// https://github.com/ianbusko/reusable-components-library

namespace simple_aspnet_auth
{
  public class LoginViewComponent : ViewComponent
  {
    public IViewComponentResult Invoke(string message)
    {
      return View();
    }
    
  }
  
}