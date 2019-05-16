using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace simple_aspnet_auth
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddAuthorization();
      services.AddScoped<IUserService, UserService>();
      services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
      services.AddMvc();
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseDeveloperExceptionPage();
      app.UseStaticFiles();
      app.UseAuthentication();
      app.UseMvcWithDefaultRoute();
    }

    public static void Main(string[] args) =>
      WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build().Run();

  }

}