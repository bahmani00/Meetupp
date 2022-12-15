using Application.Common.Interfaces;
using Application.Interfaces;
using Application.Profiles;
using Infrastructure.Photos;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Infrastructure;

public static class ConfigureServices {
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    IConfiguration configuration,
    IWebHostEnvironment env) {

    services.AddScoped<AuditEntitySaveChangesInterceptor>();

    //pass arguments into the app from the (dotnet) tools.
    //to enable a more streamlined workflow to avoid having to make manual changes to the project
    //dotnet ef migrations add MyMigration --project ../SqlServerMigrations -- --provider SqlServer
    if (configuration.Is("Provider", "Sqlite", "SqlServer")) {
      services.AddDbContext<SqliteDbContext>(options => {
        //opt.UseLazyLoadingProxies();
        options.EnableSensitiveDataLogging();
        options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
      });
      services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<SqliteDbContext>());
    } else {
      services.AddDbContext<AppDbContext>(options => {
        //opt.UseLazyLoadingProxies();
        options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
      });
      services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
    }

    services.AddScoped<DbSeeder>();

    services.AddSingleton<ISystemClock, SystemClock>();
    ////services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();

    services.AddScoped<IPhotoAccessor, PhotoAccessor>();
    services.AddScoped<IProfileReader, ProfileReader>();
    services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));

    services.AddAppIdentity(configuration, env);

    return services;
  }
}

public static class ConfigurationExt {
  public static bool Is(this IConfiguration configuration, string appKey, string value, string defaultVal) {
    var appValue = configuration.GetValue(appKey, defaultVal);
    return string.Equals(appValue, value, StringComparison.InvariantCultureIgnoreCase);
  }
}