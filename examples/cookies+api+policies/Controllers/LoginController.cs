using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

namespace simple_aspnet_auth;

using Microsoft.AspNetCore.Authentication.JwtBearer;

public class LoginController : Controller
{
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public LoginController(ITokenService tokenService, IUserService userService)
  {
    _tokenService = tokenService;
    _userService = userService;
  }


  [AllowAnonymous]
  [HttpGet("~/login")]
  public IActionResult Index() => View("_Login");

  [AllowAnonymous]
  [HttpPost("~/login")]
  public async Task<IActionResult> Login(User userViewModel)
  {
    var returnUrl = "/";

    var user = _userService.GetByName(userViewModel.Name);

    if (user.Password != userViewModel.Password)
      return LocalRedirect(returnUrl);

    var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Name) };
    var groups = user.Groups;

    foreach (var group in groups)
      claims.Add(new Claim(group.Name, group.Id.ToString()));

    var isAdmin = groups.Any(_ => _.Name == GroupNames.Admins);

    if (isAdmin)
      claims.Add(new Claim(ClaimTypes.Role, GroupNames.Admins));

    var isSuperUser = groups.Any(_ => _.Name == GroupNames.SuperUsers);

    if (isSuperUser)
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

    var returnUrl = "/";

    return LocalRedirect(returnUrl);

  }

  // Api
  [AllowAnonymous]
  [HttpPost("~/api/login")]
  public IActionResult ApiLogin(User userViewModel)
  {
    var user = _userService.GetByName(userViewModel.Name);

    if (user == null || userViewModel.Password != user.Password)
      return BadRequest();

    var claims = new[]
    {
      new Claim(ClaimTypes.Name, user.Name),
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new Claim(ClaimTypes.Role, GroupNames.Admins)
    };

    var token = _tokenService.GenerateAccessToken(claims);
    var refreshToken = _tokenService.GenerateRefreshToken();

    user.RefreshToken = refreshToken;

    _userService.Update(user);

    return new ObjectResult(new
    {
      token = token,
      refreshToken = refreshToken
    });

  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpPost("~/api/logout")]
  public IActionResult ApiLogout() => Ok();

}