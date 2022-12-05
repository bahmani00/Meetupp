using Application.Interfaces;
using Application.Profiles;
using Infrastructure.Photos;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Infrastructure;

public static class ConfigureServices {
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    IConfiguration Configuration,
    IWebHostEnvironment env) {

    //services.AddScoped<AuditableEntitySaveChangesInterceptor>();

    services.AddDbContext<DataContext>(opt => {
      //opt.UseLazyLoadingProxies();

      if (env.IsDevelopment()) {
        opt.EnableSensitiveDataLogging();
        opt.UseSqlite(
          Configuration.GetConnectionString("DefaultConnection"),
          x => x.MigrationsAssembly("Persistence.SqliteDbMigrations")
        );
      } else {
        opt.UseSqlServer(
          Configuration.GetConnectionString("DefaultConnection"),
          x => x.MigrationsAssembly("Persistence.SqlServerDbMigrations")
        );
      }
      opt.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
    });

    //services.AddScoped<DbContext>(provider => provider.GetRequiredService<DataContext>());

    ////services.AddScoped<ApplicationDbContextInitialiser>();


    services.AddSingleton<ISystemClock, SystemClock>();
    ////services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();

    services.AddScoped<IPhotoAccessor, PhotoAccessor>();
    services.AddScoped<IProfileReader, ProfileReader>();
    services.Configure<CloudinarySettings>(Configuration.GetSection("Cloudinary"));

    services.AddAppIdentity(Configuration, env);

    return services;
  }
}