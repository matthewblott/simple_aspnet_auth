using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace simple_aspnet_auth;

public class TokenController : Controller
{
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;
    
  public TokenController(ITokenService tokenService, IUserService userService)
  {
    this._tokenService = tokenService;
    this._userService = userService;
  }

  [HttpPost("~/api/token/refresh")]
  public IActionResult Refresh(string token, string refreshToken)
  {
    var principal = this._tokenService.GetPrincipalFromExpiredToken(token);
    var name = principal.Identity.Name;
    var user = this._userService.GetByName(name);

    if (user == null || user.RefreshToken != refreshToken)
      return BadRequest();

    var newToken = _tokenService.GenerateAccessToken(principal.Claims);
    var newRefreshToken = _tokenService.GenerateRefreshToken();

    user.RefreshToken = newRefreshToken;

    this._userService.Update(user);

    return new ObjectResult(new
    {
      token = newToken,
      refreshToken = newRefreshToken
    });
      
  }

  [Authorize]
  [HttpPost("~/api/token/revoke")]
  public IActionResult Revoke()
  {
    var username = User.Identity.Name;
    var user = this._userService.GetByName(username);

    if (user == null)
      return BadRequest();

    user.RefreshToken = null;

    this._userService.Update(user);

    return NoContent();

  }

}