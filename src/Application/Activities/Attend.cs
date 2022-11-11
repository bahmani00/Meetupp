using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Auth;
using Application.Errors;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities;

public static class Attend {
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

      if (activity == null)
        throw new RestException(HttpStatusCode.NotFound, new { Activity = "Cound not find activity" });

      var user = await dbContext.Users.SingleOrDefaultAsync(x =>
          x.UserName == userAccessor.GetCurrentUsername(), ct);

      var attendance = await dbContext.UserActivities
          .SingleOrDefaultAsync(x => x.ActivityId == activity.Id &&
              x.AppUserId == user.Id, ct);

      if (attendance != null)
        throw new RestException(HttpStatusCode.BadRequest,
            new { Attendance = "Already attending this activity" });

      attendance = new UserActivity {
        Activity = activity,
        AppUser = user,
        IsHost = false,
        DateJoined = DateTime.Now
      };

      dbContext.UserActivities.Add(attendance);

      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return Unit.Value;

      throw new Exception("Problem saving attendance");
    }
  }
}