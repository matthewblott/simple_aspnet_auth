using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

namespace simple_aspnet_auth;

public class LoginController : Controller
{
  private readonly IUserService _userService;

  public LoginController(IUserService userService) => _userService = userService;

  [AllowAnonymous]
  [HttpGet("~/login")]
  public async Task<IActionResult> Index() => View("_Login");

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

    claims.AddRange(groups.Select(group => new Claim(group.Name, group.Id.ToString())));

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
  public async Task<IActionResult> Logout()
  {
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    const string returnUrl = "/";

    return LocalRedirect(returnUrl);

  }

}