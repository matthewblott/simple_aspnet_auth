using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace simple_aspnet_auth
{
  public class Startup
  {
    public IConfiguration Configuration { get; set; }

    public Startup(IHostingEnvironment env)
    {
      Configuration = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json")
        .Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
      var configSection = Configuration.GetSection(nameof(JwtSettings));
      var jwtSettings = new JwtSettings();

      configSection.Bind(jwtSettings);

      services.AddSingleton<IConfiguration>(provider => Configuration);
      services.AddTransient<ITokenService, TokenService>();
      services.AddScoped<IUserService, UserService>();
      services.AddOptions();
      services.Configure<JwtSettings>(configSection);

      services.AddMvc();
      services.AddAuthentication(options =>
      {
        options.DefaultScheme = "bearer";
      }).AddJwtBearer("bearer", options =>
      {
        options.RequireHttpsMetadata = false;

        // options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidIssuer = jwtSettings.Issuer,
          ValidAudience = jwtSettings.Audience,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
          ValidateAudience = false,
          ValidateIssuer = false,
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
              context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
          }
        };

      });
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseAuthentication();
      app.UseStaticFiles();
      app.UseMvcWithDefaultRoute();

    }

  }

}
