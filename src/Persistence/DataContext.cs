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
    base.OnModelCreating(modelBuilder);

    //Define Primary Keys for UserActivity
    modelBuilder.Entity<UserActivity>(entity => {
      entity.HasKey(ua => new { ua.AppUserId, ua.ActivityId });

      //Define relationship btw UserActivity & AppUser tables
      entity.HasOne(u => u.AppUser)
        .WithMany(a => a.UserActivities)
        .HasForeignKey(u => u.AppUserId);

      //Define relationship btw UserActivity & Activity tables
      entity.HasOne(a => a.Activity)
          .WithMany(u => u.UserActivities)
          .HasForeignKey(a => a.ActivityId);
    });

    //ef fluent configuration
    modelBuilder.Entity<UserFollowing>(entity => {
      entity.HasKey(k => new { k.ObserverId, k.TargetId });

      //define many-to-many relationship

      entity.HasOne(o => o.Observer)
          .WithMany(f => f.Followings)
          .HasForeignKey(o => o.ObserverId)
          .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(o => o.Target)
          .WithMany(f => f.Followers)
          .HasForeignKey(o => o.TargetId)
          .OnDelete(DeleteBehavior.Restrict);
    });
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