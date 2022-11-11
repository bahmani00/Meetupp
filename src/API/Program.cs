using System;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace API;

public class Program {
  public static async Task Main(string[] args) {
    var host = CreateHostBuilder(args).Build();
    using (var scope = host.Services.CreateScope()) {
      var services = scope.ServiceProvider;
      try {
        var context = services.GetRequiredService<Persistence.DataContext>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        await context.Database.MigrateAsync();
        await DbSeeder.SeedAsync(context, userManager);

      } catch (Exception ex) {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error occured during MeetUppy db migration.");
        throw;
      }
    };

    host.Run();
  }

  public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => {
              webBuilder
            //turn of server header to prevent revealing the software version of the server. 
            //otherwise the server machine may become more vulnerable to attacks
            .UseKestrel(x => x.AddServerHeader = false)
            .UseStartup<Startup>();
            });
}