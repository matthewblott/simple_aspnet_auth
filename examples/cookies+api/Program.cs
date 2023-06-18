using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using simple_aspnet_auth;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
  .AddJsonFile("appsettings.json")
  .Build();

builder.WebHost.UseConfiguration(configuration);

var configSection = configuration.GetSection(nameof(JwtSettings));
var settings = new JwtSettings();

configSection.Bind(settings);

builder.Services.AddSingleton<IConfiguration>(_ => configuration);
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddOptions();
builder.Services.Configure<JwtSettings>(configSection);
builder.Services.AddMvc();
builder.Services.AddAuthentication(options =>
{
  options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
  options.RequireHttpsMetadata = false;
  options.SaveToken = true;
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidIssuer = settings.Issuer,
    ValidAudience = settings.Audience,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
    ValidateIssuerSigningKey = true,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero // the default for this setting is 5 minutes
  };
  options.Events = new JwtBearerEvents
  {
    OnAuthenticationFailed = context =>
    {
      if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
      {
        context.Response.Headers.Add("Token-Expired", true.ToString().ToLower());
      }
      return Task.CompletedTask;
    }
  };

});

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();