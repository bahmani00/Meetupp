using Application.Auth;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Application.Common.Exceptions.RestException;

namespace Application.Activities;

public static class Unattend {

  internal class Handler : IRequestHandler<Command> {
    private readonly IAppDbContext dbContext;
    private readonly ICurrUserService currUserService;

    public Handler(IAppDbContext dbContext, ICurrUserService currUserService) {
      this.dbContext = dbContext;
      this.currUserService = currUserService;
    }

    public async Task<Unit> Handle(Command request, CancellationToken ct) {
      var activity = await dbContext.Activities.FindItemAsync(request.Id, ct);
      ThrowIfNotFound(activity, new { Activity = "Not found" });

      var attendance = await dbContext.UserActivities
          .SingleOrDefaultAsync(x => x.ActivityId == activity.Id &&
              x.AppUserId == currUserService.UserId, ct);

      if (attendance == null)
        return Unit.Value;

      ThrowIfBadRequest(attendance.IsHost, new { Attendance = "You cannot remove yourself as host" });

      dbContext.UserActivities.Remove(attendance);

      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return Unit.Value;

      throw new Exception("Problem removing attandance");
    }
  }

  public record Command(Guid Id) : IRequest;
}