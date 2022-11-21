using Application.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using static Application.Errors.RestException;

namespace Application.Activities;

public static class Unattend {
  public class Command : IRequest {
    public Guid Id { get; set; }
  }

  public class Handler : IRequestHandler<Command> {
    private readonly DataContext dbContext;
    private readonly IUserAccessor userAccessor;

    public Handler(DataContext dbContext, IUserAccessor userAccessor) {
      this.dbContext = dbContext;
      this.userAccessor = userAccessor;
    }

    public async Task<Unit> Handle(Command request, CancellationToken ct) {
      var activity = await dbContext.Activities.FindItemAsync(request.Id, ct);
      ThrowIfNotFound(activity, new { Activity = "Not found" });

      var user = await userAccessor.GetCurrUserAsync();

      var attendance = await dbContext.UserActivities
          .SingleOrDefaultAsync(x => x.ActivityId == activity.Id &&
              x.AppUserId == user.Id, ct);

      if (attendance == null)
        return Unit.Value;

      ThrowIfBadRequest(attendance.IsHost, new { Attendance = "You cannot remove yourself as host" });

      dbContext.UserActivities.Remove(attendance);

      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return Unit.Value;

      throw new Exception("Problem removing attandance");
    }
  }
}