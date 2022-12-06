using Application.Common.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Application.Errors.RestException;

namespace Application.Followers;

public static class Add {

  public class Handler : IRequestHandler<Command> {
    private readonly IAppDbContext dbContext;
    private readonly IIdentityService currUserService;

    public Handler(IAppDbContext dbContext, IIdentityService currUserService) {
      this.currUserService = currUserService;
      this.dbContext = dbContext;
    }

    public async Task<Unit> Handle(Command request, CancellationToken ct) {
      var observer = await currUserService.GetCurrUserProfileAsync(ct);

      var target = await dbContext.GetUserAsync(request.Username, ct);
      ThrowIfNotFound(target, new { User = "Not found" });

      var following = await dbContext.Followings.SingleOrDefaultAsync(x => x.ObserverId == observer.Id && x.TargetId == target.Id, ct);
      ThrowIfBadRequest(following != null, new { User = "You already follow this user" });
      ThrowIfBadRequest(observer.Id == target.Id, new { User = "User canot follow itself" });

      following = new UserFollowing { ObserverId = observer.Id, TargetId = target.Id };
      dbContext.Followings.Add(following);
      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return Unit.Value;

      throw new Exception("Problem saving followX");
    }
  }

  public record Command(string Username) : IRequest;
}