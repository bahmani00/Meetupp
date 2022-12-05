using System.Globalization;
using System.Text;
using API.Middleware;
using API.Swagger;
using Application;
using Application.Auth;
using Application.Interfaces;
using Application.Profiles;
using Domain;
using FluentValidation;
using Infrastructure;
using Infrastructure.Photos;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices();

builder.WebHost
  //turn of server header to prevent revealing the software version of the server. 
  //otherwise the server machine may become more vulnerable to attacks
  // server: Kestrel  gets removed from Response headers
  .UseKestrel(x => x.AddServerHeader = false);

var app = builder.Build();
Configure();

await RunMigrationAndSeeder();

app.Run();


void ConfigureServices() {
  // Configure JSON logging to the console.
  //builder.Logging.AddJsonConsole();

  builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);

  builder.Services.AddControllers(options => {
    options.EnableEndpointRouting = false;
    // set default media type to json
    // //swagger changes also: https://stackoverflow.com/a/72367887/336511
    options.Filters.Add(new ProducesAttribute("application/json"));
  });

  builder.Services.AddSwagger();


  builder.Services.AddCors(options =>
    options.AddPolicy("CORSPolicy_React", policyBuilder =>
      policyBuilder
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials()//let signalR gets cridentials (by getting passed by websockets)
          .WithOrigins("http://localhost:3000", "https://localhost:3000")
          .WithExposedHeaders("WWW-Authenticate")
  ));

  builder.Services.AddSignalR();

  builder.Services.AddMvc(opt => {
    opt.EnableEndpointRouting = false;
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
  });

  builder.Services.AddSingleton<ISystemClock, SystemClock>();

  ConfigureIdentityServices(builder.Services);

  builder.Services.AddScoped<IPhotoAccessor, PhotoAccessor>();
  builder.Services.AddScoped<IProfileReader, ProfileReader>();
  builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
}

void ConfigureIdentityServices(IServiceCollection services) {
  var identityCoreBuilder = services.AddIdentityCore<AppUser>(opt => {
    if (builder.Environment.IsDevelopment()) {
      opt.Password.RequireDigit = false;
      opt.Password.RequireNonAlphanumeric = false;
      opt.Password.RequireUppercase = false;
      opt.Password.RequireLowercase = false;
      opt.Password.RequiredLength = 1;
    }
  });

  var identityBuilder = new IdentityBuilder(identityCoreBuilder.UserType, identityCoreBuilder.Services);
  identityBuilder.AddEntityFrameworkStores<DataContext>();
  identityBuilder.AddSignInManager<SignInManager<AppUser>>();

  services.AddAuthorization(opt => {
    opt.AddPolicy(IsHostRequirement.PolicyName, policy => {
      policy.Requirements.Add(new IsHostRequirement());
    });
  });
  services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

  services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt => {
      opt.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"])),
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
          if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat", StringComparison.OrdinalIgnoreCase))) {
            context.Token = accessToken;
          }

          return Task.CompletedTask;
        }
      };
    });

  services.AddScoped<IJwtGenerator, JwtGenerator>();
  services.AddScoped<ICurrUserService, CurrUserService>();
}

void Configure() {
  ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("fr");

  app.UseMiddleware<ErrorHandlingMiddleware>();

  //app.UseSwagger();
  ////https://editor.swagger.io/
  //app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.yaml", "API v1"));
  app.UseSwaggerAndUI();

  if (app.Environment.IsDevelopment()) {
    //app.UseDeveloperExceptionPage();
  } else {
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // Middleware to send HTTP Strict Transport Security Protocol (HSTS) headers to clients.
    //to get A ranking from SecurityHeader
    app.UseHsts();

    app.UseHttpsRedirection();//Middleware to redirect HTTP requests to HTTPS
  }

  //Configuring Content Type Options with the �nosniff� option disables MIME-type sniffing
  // to prevent attacks where files are missing metadata: X-Content-Type-Options: nosniff
  app.UseXContentTypeOptions();
  //exclude the �Referrer� header, which can improve security in cases where 
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

  app.UseRouting();
  //Authentication vs. Authorization
  app.UseAuthentication();
  app.UseAuthorization();
  app.UseCors("CORSPolicy_React");

  app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
    endpoints.MapHub<API.SignalR.ChatHub>("/chat");
  });


  //app.MapControllers();
  //app.MapHub<API.SignalR.ChatHub>("/chat", options => {
  ////https://scientificprogrammer.net/2022/09/28/advanced-signalr-configuration-fine-tuning-the-server-side-hub-and-all-supported-client-types/
  ////  options.Transports =
  ////                  HttpTransportType.WebSockets |
  ////                  HttpTransportType.LongPolling;
  ////  options.CloseOnAuthenticationExpiration = true;
  ////  options.ApplicationMaxBufferSize = 65_536;
  ////  options.TransportMaxBufferSize = 65_536;
  ////  options.MinimumProtocolVersion = 0;
  ////  options.TransportSendTimeout = TimeSpan.FromSeconds(10);
  ////  options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(3);
  ////  options.LongPolling.PollTimeout = TimeSpan.FromSeconds(10);
  ////  Console.WriteLine($"Authorization data items: {options.AuthorizationData.Count}");
  //});


  app.UseMvc(routes => {
    //when it's not /chat or api endpoints go to:
    //Configures a route that is automatically bypassed if the requested URL appears to be for a static file
    //install package Microsoft.AspNetCore.SpaServices
    routes.MapSpaFallbackRoute(
        name: "spa-fallback",
        defaults: new { controller = "Fallback", action = "Index" }
    );
  });

  //app.MapSpaFallbackRoute(x => x.);
}

async Task RunMigrationAndSeeder() {
  using (var scope = app.Services.CreateScope()) {
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
}