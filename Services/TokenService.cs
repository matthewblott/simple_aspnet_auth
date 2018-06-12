using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace simple_aspnet_auth
{
  public class TokenService : ITokenService
  {
    JwtSettings jwtSettings;

    public TokenService(IOptions<JwtSettings> settings)
    {
      this.jwtSettings = settings.Value;
    }
    
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

      var token = new JwtSecurityToken(
        issuer: jwtSettings.Issuer,
        audience: jwtSettings.Audience,
        claims: claims,
        notBefore: DateTime.UtcNow,
        expires: DateTime.UtcNow.AddMinutes(5),
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
      );

      return new JwtSecurityTokenHandler().WriteToken(token);

    }

    public string GenerateRefreshToken()
    {
      var randomNumber = new byte[32];
      using (var rng = RandomNumberGenerator.Create())
      {
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
      }
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateLifetime = false
      };

      var handler = new JwtSecurityTokenHandler();

      SecurityToken securityToken;

      var principal = handler.ValidateToken(token, tokenValidationParameters, out securityToken);

      var jwtSecurityToken = securityToken as JwtSecurityToken;

      if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
      {
        throw new SecurityTokenException("Invalid token");
      }

      return principal;

    }

  }

}