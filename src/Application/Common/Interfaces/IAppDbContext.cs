using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Common.Interfaces;

public interface IAppDbContext {
  DbSet<Activity> Activities { get; }
  DbSet<Comment> Comments { get; }
  DbSet<UserFollowing> Followings { get; }
  DbSet<Photo> Photos { get; }
  DbSet<UserActivity> UserActivities { get; }

  DbSet<AppUser> Users { get; }

  Task<int> SaveChangesAsync(CancellationToken ct = default);
  EntityEntry Remove(object entity);

  Task<AppUser?> GetUserAsync(string userId, CancellationToken ct, bool asTracking = false);
  Task<AppUser?> GetUserProfileAsync(string userId, CancellationToken ct, bool asTracking = false);
}

public static class DbSetExtensions {
  public static async ValueTask<T?> FindItemAsync<T>(this DbSet<T> set, params object[] keyValues) where T : class =>
    keyValues[^1] is CancellationToken ct ? await set.FindAsync(keyValues[0..^1], ct) : await set.FindAsync(keyValues);

  public static IQueryable<T> AsMayTracking<T>(this IQueryable<T> query, bool isTracked = false) where T : class =>
    isTracked ? query.AsTracking() : query.AsNoTracking();
}