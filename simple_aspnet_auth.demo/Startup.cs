using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace simple_aspnet_auth.demo
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddAuthorization();
      services.AddScoped<IUserService, UserService>();
      services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
      
      var myAssembly = typeof(LoginViewComponent).Assembly;

      services.AddMvc().AddApplicationPart(myAssembly);

      services.Configure<RazorViewEngineOptions>(options =>
      {
//        options.FileProviders.Add(new EmbeddedFileProvider(myAssembly, nameof(simple_aspnet_auth)));          
      });
      
      services.AddMvc(options =>
        {
          options.EnableEndpointRouting = true;
        })
        .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseDeveloperExceptionPage();
      app.UseStaticFiles();
      app.UseRouting();
      app.UseAuthentication();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute("default", "{controller}/{action}/{id?}", 
          defaults: new { controller = "Home", action = "Index" });
        endpoints.MapRazorPages();
      });

    }

    public static void Main(string[] args) =>
      WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build().Run();

  }

}