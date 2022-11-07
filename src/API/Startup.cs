using System;
using System.Text;
using System.Threading.Tasks;
using API.Middleware;
using Application;
using Application.Auth;
using Application.Interfaces;
using Application.Profiles;
using Domain;
using Infrastructure.Photos;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence;

namespace API;

public class Startup {
  public Startup(IConfiguration configuration) {
    Configuration = configuration;
  }

  private readonly IConfiguration Configuration;

  public void ConfigureDevelopmentServices(IServiceCollection services) {

    services.AddApplicationServices(Configuration);

    ConfigureServices(services);
  }

  public void ConfigureProductionServices(IServiceCollection services) {
    services.AddDbContext<DataContext>(opt => {
      opt.UseLazyLoadingProxies();
      opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
              x => x.MigrationsAssembly("Persistence.SqlServerDbMigrations"));
      opt.UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }));
    });

    ConfigureServices(services);
  }

  // This method gets called by the runtime. Use this method to add services to the container.
  public void ConfigureServices(IServiceCollection services) {
    services.AddControllers(options => {
      options.EnableEndpointRouting = false;
      // set default media type to json
      // //swagger changes also: https://stackoverflow.com/a/72367887/336511
      options.Filters.Add(new ProducesAttribute("application/json"));
    });
    services.AddSwaggerGen(options => {
      options.SwaggerDoc("v1", new OpenApiInfo { Title = "MeetUppy API", Version = "v1" });

      // UseFullTypeNameInSchemaIds replacement for .NET Core
      options.CustomSchemaIds(x => x.FullName.Replace("+", "."));

      //https://www.infoworld.com/article/3650668/implement-authorization-for-swagger-in-aspnet-core-6.html
      options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
      });

      options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
          new OpenApiSecurityScheme {
            Reference = new OpenApiReference {
              Type = ReferenceType.SecurityScheme,
              Id = "Bearer"
            }
          },
          Array.Empty<string>()
        }
      });
    });

    services.AddCors(options =>
        options.AddPolicy("CORSPolicy_React", policyBuilder =>
            policyBuilder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()//let signalR gets cridentials (by getting passed by websockets)
                .WithOrigins("http://localhost:3000", "https://localhost:3000")
                .WithExposedHeaders("WWW-Authenticate")
    ));


    services.AddSignalR();
    services.AddMvc(opt => {
      opt.EnableEndpointRouting = false;
      var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
      opt.Filters.Add(new AuthorizeFilter(policy));
    });

    services.AddSingleton<ISystemClock, SystemClock>();
    var builder = services.AddIdentityCore<AppUser>(opt => {              
      // opt.Password.RequireDigit = false;
      // opt.Password.RequireNonAlphanumeric = false;
      // opt.Password.RequireUppercase = false;
      // opt.Password.RequireLowercase = false;
      // opt.Password.RequiredLength = 1;
    });

    var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
    identityBuilder.AddEntityFrameworkStores<DataContext>();
    identityBuilder.AddSignInManager<SignInManager<AppUser>>();

    services.AddAuthorization(opt => {
      opt.AddPolicy("IsHostCreatedActivity", policy => {
        policy.Requirements.Add(new IsHostRequirement());
      });
    });
    services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opt => {
          opt.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"])),
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
                  if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat"))) {
                    context.Token = accessToken;
                  }

                  return Task.CompletedTask;
                }
          };
        });

    services.AddScoped<IJwtGenerator, JwtGenerator>();
    services.AddScoped<IUserAccessor, UserAccessor>();

    services.AddScoped<IPhotoAccessor, PhotoAccessor>();
    services.AddScoped<IProfileReader, ProfileReader>();
    services.Configure<CloudinarySettings>(Configuration.GetSection("Cloudinary"));
  }

  // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
  public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
    app.UseMiddleware<ErrorHandlingMiddleware>();

    if (env.IsDevelopment()) {
      //app.UseDeveloperExceptionPage();
      app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "API v1"));
    } else {
      // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
      // Middleware to send HTTP Strict Transport Security Protocol (HSTS) headers to clients.
      //to get A ranking from SecurityHeader
      app.UseHsts();

      app.UseHttpsRedirection();//Middleware to redirect HTTP requests to HTTPS
    }

    app.UseRouting();

    //Configuring Content Type Options with the ‘nosniff’ option disables MIME-type sniffing
    // to prevent attacks where files are missing metadata: X-Content-Type-Options: nosniff
    app.UseXContentTypeOptions();
    //exclude the ‘Referrer’ header, which can improve security in cases where 
    //the URL of the previous web page contains sensitive data.Referrer-Policy: no-referrer
    app.UseReferrerPolicy(opt => opt.NoReferrer());
    //enables the detection of XSS attacks: X-XSS-Protection: 1; mode=block
    app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
    //prevent click-jacking attacks: X-Frame-Options: Deny
    app.UseXfo(opt => opt.Deny());
    //Content Security Policy header,allows you to configure at a very granular level what content 
    //you want to allow your web app to load and precisely which sources you want to load content from.
    //use app.UseCspReportOnly to get the reports
    app.UseCsp(opt => opt
            .BlockAllMixedContent()
            .StyleSources(s => s.Self()
                .CustomSources("https://fonts.googleapis.com", "sha256-F4GpCPyRepgP5znjMD8sc7PEjzet5Eef4r09dEGPpTs="))
            .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com", "data:"))
            .FormActions(s => s.Self())
            .FrameAncestors(s => s.Self())
            //to fix react-cropper issue: "blob:", "data:"
            .ImageSources(s => s.Self().CustomSources("https://res.cloudinary.com", "blob:", "data:"))
        //tried this but couldn't fix
        //.ScriptSources(s => s.Self().CustomSources("sha256-ma5XxS1EBgt17N22Qq31rOxxRWRfzUTQS1KOtfYwuNo="))
        );

    app.UseDefaultFiles();//enable index.html,default.htm,...
    app.UseStaticFiles();//static files: js, css, img,...

    //Authentication vs. Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints => {
      endpoints.MapHub<SignalR.ChatHub>("/chat");
    });

    app.UseCors("CORSPolicy_React");

    app.UseMvc(routes => {
      //when it's not /chat or api endpoints go to:
      //Configures a route that is automatically bypassed if the requested URL appears to be for a static file
      //install package Microsoft.AspNetCore.SpaServices
      routes.MapSpaFallbackRoute(
          name: "spa-fallback",
          defaults: new { controller = "Fallback", action = "Index" }
      );
    });

    //app.MapFallbackToFile("index.html"); ;

  }
}
