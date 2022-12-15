using System.Globalization;
using API;
using API.Middleware;
using API.Swagger;
using Application;
using Application.Common.Interfaces;
using FluentValidation;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices();

builder.WebHost
  //turn of server header to prevent revealing the software version of the server. 
  //otherwise the server machine may become more vulnerable to attacks
  // server: Kestrel  gets removed from Response headers
  .UseKestrel(x => x.AddServerHeader = false);

var app = builder.Build();

await RunMigrationAndSeeder();

Configure();

app.Run();


void ConfigureServices() {
  // Configure JSON logging to the console.
  //builder.Logging.AddJsonConsole();

  builder.Services.AddApplicationServices();
  builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);

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

    app.ApplySecurityHeaders();
  }

  app.UseCors("CORSPolicy_React");

  app.UseDefaultFiles();//enable index.html,default.htm,...
  app.UseStaticFiles();//static files: js, css, img,...

  app.UseRouting();
  //Authentication vs. Authorization
  app.UseAuthentication();
  app.UseAuthorization();

  app.MapControllers();
  app.UseEndpoints(endpoints => {
    endpoints.MapHub<API.SignalR.ChatHub>("/chat", options => {
      //https://scientificprogrammer.net/2022/09/28/advanced-signalr-configuration-fine-tuning-the-server-side-hub-and-all-supported-client-types/
      //  options.Transports =
      //                  HttpTransportType.WebSockets |
      //                  HttpTransportType.LongPolling;
      //  options.CloseOnAuthenticationExpiration = true;
      //  options.ApplicationMaxBufferSize = 65_536;
      //  options.TransportMaxBufferSize = 65_536;
      //  options.MinimumProtocolVersion = 0;
      //  options.TransportSendTimeout = TimeSpan.FromSeconds(10);
      //  options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(3);
      //  options.LongPolling.PollTimeout = TimeSpan.FromSeconds(10);
      //  Console.WriteLine($"Authorization data items: {options.AuthorizationData.Count}");
      //});
    });
  });

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
  using var scope = app.Services.CreateScope();
  var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
  await (dbContext as DbContext)!.Database.MigrateAsync();

  var dbSeeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
  await dbSeeder.SeedAsync();
}