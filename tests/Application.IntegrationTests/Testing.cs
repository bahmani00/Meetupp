using Domain;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Respawn;

namespace Application.IntegrationTests;

//[SetUpFixture]
public partial class Testing : IDisposable {
  private static WebApplicationFactory<Program> _factory = null!;
  private static IConfiguration _configuration = null!;
  private static IServiceScopeFactory _scopeFactory = null!;
  private static Checkpoint _checkpoint = null!;
  private static string? _currentUserId;

  public Testing() {
    // OneTimeSetUp: Do "global" initialization here; Called before every test method.
    RunBeforeAnyTests();
  }

  //[OneTimeSetUp]
  public void RunBeforeAnyTests() {
    _factory = new CustomWebApplicationFactory();
    _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
    _configuration = _factory.Services.GetRequiredService<IConfiguration>();

    _checkpoint = new Checkpoint {
      TablesToIgnore = new[] { "__EFMigrationsHistory" }
    };
  }

  public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request) {
    using var scope = _scopeFactory.CreateScope();

    var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

    return await mediator.Send(request);
  }

  public static string? GetCurrentUserId() {
    return _currentUserId;
  }

  public static async Task<string> RunAsDefaultUserAsync() {
    return await RunAsUserAsync("test@local", "Testing1234!", Array.Empty<string>());
  }

  public static async Task<string> RunAsAdministratorAsync() {
    return await RunAsUserAsync("administrator@local", "Administrator1234!", new[] { "Administrator" });
  }

  public static async Task<string> RunAsUserAsync(string userName, string password, string[] roles) {
    using var scope = _scopeFactory.CreateScope();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    var user = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName);
    if (user != null) {
      return _currentUserId = user.Id;
    }

    user = new AppUser { UserName = userName, Email = userName, DisplayName = userName };
    var result = await userManager.CreateAsync(user, password);

    if (roles.Any()) {
      var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

      foreach (var role in roles) {
        await roleManager.CreateAsync(new IdentityRole(role));
      }

      await userManager.AddToRolesAsync(user, roles);
    }

    if (result.Succeeded) {
      return _currentUserId = user.Id;
    }

    var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

    throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
  }

  public static async Task ResetState() {
    await _checkpoint.Reset(_configuration.GetConnectionString("DefaultConnection"));

    _currentUserId = null;
  }

  public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
      where TEntity : class {
    using var scope = _scopeFactory.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    return await context.FindAsync<TEntity>(keyValues);
  }

  public static async Task AddAsync<TEntity>(TEntity entity)
      where TEntity : class {
    using var scope = _scopeFactory.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Add(entity);

    await context.SaveChangesAsync();
  }

  public static async Task<int> CountAsync<TEntity>() where TEntity : class {
    using var scope = _scopeFactory.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    return await context.Set<TEntity>().CountAsync();
  }

  //[OneTimeTearDown]
  public void RunAfterAnyTests() {
  }

  public void Dispose() {
    // "global" teardown here; Called after every test method.
  }
}