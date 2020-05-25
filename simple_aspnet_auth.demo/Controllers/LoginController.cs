using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

namespace simple_aspnet_auth.demo
{
  public class LoginController : Controller
  {
    private readonly IUserService _userService;

    public LoginController(IUserService userService) => _userService = userService;

    [AllowAnonymous]
    [HttpGet("~/login")]
    public IActionResult Index() => View();

    [AllowAnonymous]
    [HttpPost("~/login")]
    public async Task<IActionResult> Login(User userViewModel)
    {
      const string returnUrl = "/";

      var user = _userService.GetByName(userViewModel.Name);

      if (user.Password != userViewModel.Password)
        return LocalRedirect(returnUrl);

      var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Name) };
      var groups = user.Groups;

      foreach (var group in groups)
        claims.Add(new Claim(group.Name, group.Id.ToString()));

      var isAdmin = groups.Any(_ => _.Name == GroupNames.Admins);

      if(isAdmin)
        claims.Add(new Claim(ClaimTypes.Role, GroupNames.Admins));

      var isSuperUser = groups.Any(_ => _.Name == GroupNames.SuperUsers);

      if(isSuperUser)
        claims.Add(new Claim(ClaimTypes.Role, GroupNames.SuperUsers));

      var props = new AuthenticationProperties
      {
        IsPersistent = true,
        ExpiresUtc = DateTime.UtcNow.AddMinutes(5),
      };

      var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
      var principal = new ClaimsPrincipal(identity);

      await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

      return LocalRedirect(returnUrl);

    }

    [HttpPost("~/logout")]
    public IActionResult Logout()
    {
      HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

      const string returnUrl = "/";

      return LocalRedirect(returnUrl);

    }

  }

}