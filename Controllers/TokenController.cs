using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace simple_aspnet_auth
{
  public class TokenController : Controller
  {
    ITokenService tokenService;
    IUserService userService;
    public TokenController(ITokenService tokenService, IUserService userService)
    {
      this.tokenService = tokenService;
      this.userService = userService;
    }

    [HttpPost]
    public IActionResult Refresh(string token, string refreshToken)
    {
      var principal = this.tokenService.GetPrincipalFromExpiredToken(token);
      var username = principal.Identity.Name;
      var user = this.userService.GetByName(username);

      if (user == null || user.RefreshToken != refreshToken)
        return BadRequest();

      var newJwtToken = tokenService.GenerateAccessToken(principal.Claims);
      var newRefreshToken = tokenService.GenerateRefreshToken();

      user.RefreshToken = newRefreshToken;

      this.userService.Update(user);

      return new ObjectResult(new
      {
        token = newJwtToken,
        refreshToken = newRefreshToken
      });
      
    }

    [HttpPost]
    [Authorize]
    public IActionResult Revoke()
    {
      var username = User.Identity.Name;
      var user = this.userService.GetByName(username);

      if (user == null)
        return BadRequest();

      user.RefreshToken = null;

      this.userService.Update(user);

      return NoContent();

    }

  }
  
}