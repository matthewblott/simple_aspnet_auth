namespace simple_aspnet_auth;

public class JwtSettings
{
  public string Key { get; set; }
  public string Issuer { get; set; }
  public string Audience { get; set; }
  public int AccessTokenDurationInMinutes { get; set; }
}