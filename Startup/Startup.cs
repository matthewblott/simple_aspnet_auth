using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace simple_aspnet_auth
{
  public class Startup
  {
		public IConfiguration Configuration { get; }

		public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
      
			Configuration = builder.Build();

		}

    public void ConfigureServices(IServiceCollection services)
    {
			var configSection = Configuration.GetSection(nameof(JwtSettings));
			var jwtSettings = new JwtSettings();

			configSection.Bind(jwtSettings);

			services.AddAuthorization(options =>
			{
				options.AddPolicy(GroupNames.Admins, policy => { policy.Requirements.Add(new AdminRequirement()); });
				options.AddPolicy(GroupNames.SuperUsers, policy => { policy.Requirements.Add(new AdminRequirement(true)); });
			});
			services.AddSingleton<IAuthorizationHandler, AdminHandler>();
			services.AddOptions();
      services.Configure<JwtSettings>(configSection);
			services.AddScoped<IUserService, UserService>();
			services.AddAuthentication(x =>
      {
        x.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        x.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        x.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      })
      .AddCookie(x =>
      {
        x.LoginPath = new PathString("/Home/Login/");
        x.AccessDeniedPath = new PathString("/Shared/Error/");
        x.SlidingExpiration = true;
      }).AddJwtBearer(x =>
			{
				x.RequireHttpsMetadata = false;
				x.SaveToken = true;
				x.TokenValidationParameters = new TokenValidationParameters()
				{
          ValidIssuer = jwtSettings.Issuer,
          ValidAudience = jwtSettings.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
					ClockSkew = TimeSpan.Zero,
				};
			});
			services.AddMvc();

		}

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStaticFiles();
			app.UseAuthentication();

			app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "default",
          template: "{controller=Home}/{action=Index}/{id?}");
      });

    }

  }

}