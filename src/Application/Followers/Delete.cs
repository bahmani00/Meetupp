using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Application.Common.Exceptions.RestException;

namespace Application.Followers;

public static class Delete {

  internal class Handler : IRequestHandler<Command> {
    private readonly IAppDbContext dbContext;
    private readonly IIdentityService currUserService;

    public Handler(IAppDbContext dbContext, IIdentityService currUserService) {
      this.dbContext = dbContext;
      this.currUserService = currUserService;
    }

    public async Task<Unit> Handle(Command request, CancellationToken ct) {
      var observer = await currUserService.GetCurrUserProfileAsync(ct);

      var target = await dbContext.GetUserAsync(request.Username, ct);
      ThrowIfNotFound(target, new { User = "Not found" });

      var following = await dbContext.Followings.TagWithCallSite().SingleOrDefaultAsync(x => x.ObserverId == observer.Id && x.TargetId == target!.Id, ct);
      ThrowIfBadRequest(following == null, new { User = "You are not following this user" });

      dbContext.Followings.Remove(following!);
      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return Unit.Value;

      throw new Exception("Problem saving changes");
    }
  }

  public record Command(string Username) : IRequest;

}