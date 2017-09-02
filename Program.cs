using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace simple_aspnet_auth
{
  public class Program
  {
    public static void Main(string[] args)
    {
			var config = new ConfigurationBuilder()
			  .SetBasePath(Directory.GetCurrentDirectory())
			  .AddJsonFile("hosting.json", optional: true)
			  .Build();

      var builder = new WebHostBuilder()
        .UseKestrel()
        .UseConfiguration(config)
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseStartup<Startup>();

			try
      {
        var host = builder.Build();

				host.Run();

			}
      catch(Exception ex)
      {
        Console.WriteLine(ex.Message);
        Console.ReadLine();
      }

		}

  }

}