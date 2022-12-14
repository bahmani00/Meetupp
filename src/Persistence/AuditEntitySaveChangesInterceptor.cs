using Application.Auth;
using Domain.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Persistence;

public class AuditEntitySaveChangesInterceptor : SaveChangesInterceptor {
  private readonly ICurrUserService currUserService;
  private readonly ISystemClock systemClock;

  public AuditEntitySaveChangesInterceptor(ICurrUserService currUserService, ISystemClock systemClock) {
    this.currUserService = currUserService;
    this.systemClock = systemClock;
  }

  public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result) {
    UpdateEntities(eventData.Context!);

    return base.SavingChanges(eventData, result);
  }

  public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default) {
    UpdateEntities(eventData.Context!);

    return base.SavingChangesAsync(eventData, result, ct);
  }

  public void UpdateEntities(DbContext context) {
    if (currUserService.UserId == null) return;

    foreach (var entry in context.ChangeTracker.Entries<Entity>()) {
      if (entry.State == EntityState.Added) {
        entry.Entity.CreatedById = currUserService.UserId;
        entry.Entity.CreatedOn = systemClock.UtcNow.UtcDateTime;
      } else if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities()) {
        entry.Entity.ModifiedById = currUserService.UserId;
        entry.Entity.ModifiedOn = systemClock.UtcNow.UtcDateTime;
      }
    }
  }
}

public static class Extensions {
  public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
      entry.References.Any(r =>
          r.TargetEntry != null &&
          r.TargetEntry.Metadata.IsOwned() &&
          (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}