using System.Collections.Generic;
using System.Security.Claims;

namespace simple_aspnet_auth
{
  public interface ITokenService
  {
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
  }
}