using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace simple_aspnet_auth;

public class TokenService : ITokenService
{
  private readonly JwtSettings _settings;

  public TokenService(IOptions<JwtSettings> settings) => this._settings = settings.Value;

  public string GenerateAccessToken(IEnumerable<Claim> claims)
  {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));

    var token = new JwtSecurityToken(
      issuer: _settings.Issuer,
      audience: _settings.Audience,
      claims: claims,
      notBefore: DateTime.UtcNow,
      expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenDurationInMinutes),
      signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
    );

    return new JwtSecurityTokenHandler().WriteToken(token);

  }

  public string GenerateRefreshToken()
  {
    var number = new byte[32];

    using var generator = RandomNumberGenerator.Create();
    generator.GetBytes(number);

    return Convert.ToBase64String(number);
  }

  public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
  {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));

    var tokenValidationParameters = new TokenValidationParameters
    {
      ValidateAudience = false,
      ValidateIssuer = false,
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = key,
      ValidateLifetime = false,
    };

    var handler = new JwtSecurityTokenHandler();

    var principal = handler.ValidateToken(token, tokenValidationParameters, out var securityToken);

    var jwtSecurityToken = securityToken as JwtSecurityToken;

    if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
    {
      throw new SecurityTokenException("Invalid token");
    }

    return principal;

  }

}