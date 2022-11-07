﻿using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application;

public static class ConfigureServices {
  public static IServiceCollection AddApplicationServices(
    this IServiceCollection services,
    IConfiguration Configuration) {
      
    services.AddDbContext<DataContext>(opt => {
      opt.UseLazyLoadingProxies();
      opt.EnableSensitiveDataLogging();
      opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("Persistence.SqliteDbMigrations"));
      //opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
      //        x => x.MigrationsAssembly("Persistence.SqlServerDbMigrations"));
      opt.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
    });
    services.AddAutoMapper(Assembly.GetExecutingAssembly());
    services.AddMediatR(Assembly.GetExecutingAssembly());
    services.AddFluentValidationAutoValidation();
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    
    //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
    //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
    //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
    //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

    return services;
  }
}