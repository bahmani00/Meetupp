using Application.Activities;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Persistence;
using MediatR;
using FluentValidation.AspNetCore;
using API.Middleware;
using Domain;
using Microsoft.AspNetCore.Authentication;
using Application.Auth;
using Infrastructure.Security;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Logging;
using Application.Interfaces;
using Infrastructure.Photos;
using System.Threading.Tasks;
using Application.Profiles;
using System;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseLazyLoadingProxies();
                opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"),
                         x => x.MigrationsAssembly("Persistence.SqliteDbMigrations"));
                //opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                //        x => x.MigrationsAssembly("Persistence.SqlServerDbMigrations"));
                opt.UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }));                
            });

            ConfigureServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseLazyLoadingProxies();
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                        x => x.MigrationsAssembly("Persistence.SqlServerDbMigrations"));
                opt.UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }));                
            });

            ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => options.EnableEndpointRouting = false);
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MeetUppy API", Version = "v1" });
                //c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                // UseFullTypeNameInSchemaIds replacement for .NET Core
                c.CustomSchemaIds(x => x.FullName);
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

            services.AddMediatR(typeof(List.Query).Assembly);
            services.AddAutoMapper(typeof(List.Handler));
            services.AddSignalR();
            services.AddMvc(opt =>
            {
                opt.EnableEndpointRouting = false;
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Create>());
            
            services.AddSingleton<ISystemClock, SystemClock>();
            var builder = services.AddIdentityCore<AppUser>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            identityBuilder.AddEntityFrameworkStores<DataContext>();
            identityBuilder.AddSignInManager<SignInManager<AppUser>>();

            services.AddAuthorization(opt => 
            {
                opt.AddPolicy("IsHostCreatedActivity", policy =>
                {
                    policy.Requirements.Add(new IsHostRequirement());
                });
            });
            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"])),
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true, //validate expired tokens: user get 401 unauthorized status
                        ClockSkew = TimeSpan.Zero
                    };

                    //add auth token to HubContext
                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>  //MessageReceivedContext
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat")))
                            {
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();
            
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "API v1"));
            }
            else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
                //app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseDefaultFiles();//enable index.html,default.htm,...
            app.UseStaticFiles();//static files: js, css, img,...
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("CORSPolicy_React");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SignalR.ChatHub>("/chat");
            });
			
            app.UseMvc(routes => 
            {
				//when it's not /chat or api endpoints go to:
                //Configures a route that is automatically bypassed if the requested URL appears to be for a static file
                //install package Microsoft.AspNetCore.SpaServices
                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new {controller = "Fallback", action = "Index"}
                );
            });
        }
    }
}
