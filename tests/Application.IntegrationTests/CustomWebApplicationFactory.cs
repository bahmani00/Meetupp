using Application.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Persistence;

namespace Application.IntegrationTests;

using static Testing;

internal class CustomWebApplicationFactory : WebApplicationFactory<Program> {
  protected override void ConfigureWebHost(IWebHostBuilder builder) {
    builder.ConfigureAppConfiguration(configurationBuilder => {
      var integrationConfig = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json")
          .AddEnvironmentVariables()
          .Build();

      configurationBuilder.AddConfiguration(integrationConfig);
    });

    builder.ConfigureServices((builder, services) => {
      services
          .Remove<ICurrUserService>()
          .AddTransient(provider => Mock.Of<ICurrUserService>(s =>
              s.UserId == GetCurrentUserId()));

      services
          .Remove<DbContextOptions<AppDbContext>>()
          .AddDbContext<AppDbContext>((sp, options) =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                  builder => builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
    });
  }
}