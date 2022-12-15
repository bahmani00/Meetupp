using System.Reflection;
using Application.Common.Interfaces;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence;


public class AppDbContext : IdentityDbContext<AppUser>, IAppDbContext {
  protected readonly AuditEntitySaveChangesInterceptor auditEntitySaveChangesInterceptor;
  protected readonly IConfiguration configuration;

  public AppDbContext(
    DbContextOptions options,
    AuditEntitySaveChangesInterceptor auditInterceptor,
    IConfiguration configuration) : base(options) {
    this.auditEntitySaveChangesInterceptor = auditInterceptor;
    this.configuration = configuration;
  }

  //TODO: remove these sets. configurations will add them
  public DbSet<Activity> Activities => Set<Activity>();
  public DbSet<UserActivity> UserActivities => Set<UserActivity>();
  public DbSet<Photo> Photos => Set<Photo>();
  public DbSet<Comment> Comments => Set<Comment>();
  public DbSet<UserFollowing> Followings => Set<UserFollowing>();

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    base.OnModelCreating(modelBuilder);
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    optionsBuilder.UseSqlServer(
      configuration.GetConnectionString("SqlServerConnection"),
      x => x.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));

    optionsBuilder.AddInterceptors(auditEntitySaveChangesInterceptor);
  }

  /// <inheritdoc />
  public async Task<AppUser?> GetUserAsync(string userName, CancellationToken ct, bool asTracking = false) =>
    await Users
      .AsMayTracking(asTracking)
      .SingleOrDefaultAsync(x => x.UserName == userName, ct);

  /// <inheritdoc />
  public async Task<AppUser?> GetUserProfileAsync(string userName, CancellationToken ct, bool asTracking = false) =>
    await Users
      .AsMayTracking(asTracking)
      .Include(x => x.Followings)
      .Include(x => x.Followers)
      .Include(x => x.Photos)
      .SingleOrDefaultAsync(x => x.UserName == userName, ct);
}