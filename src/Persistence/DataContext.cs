using System.Reflection;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class DataContext : IdentityDbContext<AppUser> {
  public DataContext(DbContextOptions options) : base(options) {
  }

  public DbSet<Activity> Activities { get; set; }
  public DbSet<UserActivity> UserActivities { get; set; }
  public DbSet<Photo> Photos { get; set; }
  public DbSet<Comment> Comments { get; set; }
  public DbSet<UserFollowing> Followings { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    
    base.OnModelCreating(modelBuilder);
  }

  public async Task<AppUser> GetUserAsync(string userName, CancellationToken ct, bool asTracking = false) =>
    await Users
      .AsMayTracking(asTracking)
      .SingleOrDefaultAsync(x => x.UserName == userName, ct);

  public async Task<AppUser> GetUserProfileAsync(string userName, CancellationToken ct, bool asTracking = false) =>
    await Users
      .AsMayTracking(asTracking)
      .Include(x => x.Followings)
      .Include(x => x.Followers)
      .Include(x => x.Photos)
      .SingleOrDefaultAsync(x => x.UserName == userName, ct);
}

public static class DbSetExtensions {
  public static async ValueTask<T> FindItemAsync<T>(this DbSet<T> set, params object[] keyValues) where T : class =>
    keyValues[^1] is CancellationToken ct ? await set.FindAsync(keyValues[0..^1], ct) : await set.FindAsync(keyValues);

  public static IQueryable<T> AsMayTracking<T>(this IQueryable<T> query, bool isTracked = false) where T : class =>
    isTracked ? query.AsTracking() : query.AsNoTracking();
}