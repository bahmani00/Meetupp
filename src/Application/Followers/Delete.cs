using Application.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using static Application.Errors.RestException;

namespace Application.Followers;

public static class Delete {
  public class Command : IRequest {
    public string Username { get; set; }
  }

  public class Handler : IRequestHandler<Command> {
    private readonly DataContext dbContext;
    private readonly ICurrUserService currUserService;

    public Handler(DataContext dbContext, ICurrUserService currUserService) {
      this.dbContext = dbContext;
      this.currUserService = currUserService;
    }

    public async Task<Unit> Handle(Command request, CancellationToken ct) {
      var observer = await currUserService.GetCurrUserAsync();

      var target = await dbContext.GetUserAsync(request.Username, ct);
      ThrowIfNotFound(target, new { User = "Not found" });

      var following = await dbContext.Followings.SingleOrDefaultAsync(x => x.ObserverId == observer.Id && x.TargetId == target.Id, ct);
      ThrowIfBadRequest(following == null, new { User = "You are not following this user" });

      dbContext.Followings.Remove(following);
      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return Unit.Value;

      throw new Exception("Problem saving changes");
    }
  }
}