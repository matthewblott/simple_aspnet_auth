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

    [HttpPost("~/api/token/refresh")]
    public IActionResult Refresh(string token, string refreshToken)
    {
      var principal = this.tokenService.GetPrincipalFromExpiredToken(token);
      var name = principal.Identity.Name;
      var user = this.userService.GetByName(name);

      if (user == null || user.RefreshToken != refreshToken)
        return BadRequest();

      var newToken = tokenService.GenerateAccessToken(principal.Claims);
      var newRefreshToken = tokenService.GenerateRefreshToken();

      user.RefreshToken = newRefreshToken;

      this.userService.Update(user);

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
      var user = this.userService.GetByName(username);

      if (user == null)
        return BadRequest();

      user.RefreshToken = null;

      this.userService.Update(user);

      return NoContent();

    }

  }
  
}