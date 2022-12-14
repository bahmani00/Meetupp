using System.Text;
using Application.Auth;
using Application.Common.Interfaces;
using Domain;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace Infrastructure.Security;

public static class ConfigureServices {
  public static IServiceCollection AddAppIdentity(
    this IServiceCollection services,
    IConfiguration Configuration,
    IWebHostEnvironment env) {

    //services
    //    .AddDefaultIdentity<AppUser>()
    //    .AddRoles<IdentityRole>()
    //    .AddEntityFrameworkStores<ApplicationDbContext>();

    //services.AddIdentityServer()
    //    .AddApiAuthorization<AppUser, ApplicationDbContext>();

    services.AddTransient<IIdentityService, IdentityService>();

    //services.AddAuthentication()
    //    .AddIdentityServerJwt();

    //services.AddAuthorization(options =>
    //    options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator")));




    var identityCoreBuilder = services.AddIdentityCore<AppUser>(opt => {
      if (env.IsDevelopment()) {
        opt.Password.RequireDigit = false;
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireLowercase = false;
        opt.Password.RequiredLength = 1;
      }
    });

    var identityBuilder = new IdentityBuilder(identityCoreBuilder.UserType, identityCoreBuilder.Services);
    identityBuilder.AddEntityFrameworkStores<AppDbContext>();
    identityBuilder.AddSignInManager<SignInManager<AppUser>>();

    services.AddAuthorization(opt => {
      opt.AddPolicy(IsHostRequirement.PolicyName, policy => {
        policy.Requirements.Add(new IsHostRequirement());
      });
    });
    //services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(opt => {
        opt.TokenValidationParameters = new TokenValidationParameters {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"]!)),
          ValidateAudience = false,
          ValidateIssuer = false,
          ValidateLifetime = true, //validate expired tokens: user get 401 unauthorized status
          ClockSkew = TimeSpan.Zero
        };

        //add auth token to HubContext
        opt.Events = new JwtBearerEvents {
          OnMessageReceived = context =>  //MessageReceivedContext
          {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat", StringComparison.OrdinalIgnoreCase)) {
              context.Token = accessToken;
            }

            return Task.CompletedTask;
          }
        };
      });

    services.AddScoped<IJwtGenerator, JwtGenerator>();
    services.AddScoped<ICurrUserService, CurrUserService>();

    return services;
  }
}