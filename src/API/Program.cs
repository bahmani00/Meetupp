using System;
using API;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => {
      //turn of server header to prevent revealing the software version of the server. 
      //otherwise the server machine may become more vulnerable to attacks
      webBuilder
        .UseKestrel(x => x.AddServerHeader = false)
        .UseStartup<Startup>();
    }).Build();

using (var scope = host.Services.CreateScope()) {
  var services = scope.ServiceProvider;
  try {
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await DbSeeder.SeedAsync(context, userManager);

  } catch (Exception ex) {
    ILogger logger = services.GetRequiredService<ILogger<Program>>();
    logger.Error("Error occured during MeetUppy db migration.", ex);
    throw;
  }
};

host.Run();