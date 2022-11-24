using Application.Auth;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using static Application.Errors.RestException;

namespace Application.Followers;

public static class Add {
  public class Command : IRequest {
    public string Username { get; set; }
  }

  public class Handler : IRequestHandler<Command> {
    private readonly DataContext dbContext;
    private readonly ICurrUserService currUserService;

    public Handler(DataContext dbContext, ICurrUserService currUserService) {
      this.currUserService = currUserService;
      this.dbContext = dbContext;
    }

    public async Task<Unit> Handle(Command request, CancellationToken ct) {
      var observer = await currUserService.GetCurrUserAsync();

      var target = await dbContext.GetUserAsync(request.Username, ct);
      ThrowIfNotFound(target, new { User = "Not found" });

      var following = await dbContext.Followings.SingleOrDefaultAsync(x => x.ObserverId == observer.Id && x.TargetId == target.Id, ct);
      ThrowIfBadRequest(following != null, new { User = "You already follow this user" });

      following = new UserFollowing { ObserverId = observer.Id, TargetId = target.Id };
      dbContext.Followings.Add(following);
      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return Unit.Value;

      throw new Exception("Problem saving followX");
    }
  }
}