using System.Linq.Expressions;
using Application.Common.Interfaces;
using Application.Profiles;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Followers;

public static class List {

  public class Handler : IRequestHandler<Query, List<Profile>> {
    private readonly IAppDbContext dbContext;
    private readonly IIdentityService currUserService;

    public Handler(IAppDbContext dbContext, IIdentityService currUserService) {
      this.dbContext = dbContext;
      this.currUserService = currUserService;
    }

    public async Task<List<Profile>> Handle(Query request, CancellationToken ct) {
      var currUser = await currUserService.GetCurrUserProfileAsync(ct);
      Expression<Func<UserFollowing, bool>> predicate = (x) => x.TargetId == request.UserId;
      Expression<Func<UserFollowing, AppUser>> includePerdicate = x => x.Observer;
      if (request.IsFollowing()) {
        predicate = (x) => x.ObserverId == request.UserId;
        includePerdicate = x => x.Target;
      }

      var userFollowings = await dbContext.Followings
        //.AsNoTracking() // to prevent cycling
        .Include(includePerdicate).ThenInclude(x => x.Photos)
        .Include(includePerdicate).ThenInclude(x => x.Followers)
        .Include(includePerdicate).ThenInclude(x => x.Followings)
        .Where(predicate)
        .TagWithCallSite()
        .ToListAsync(ct);

      var getUser = includePerdicate.Compile();
      return userFollowings.Select(x => Profile.From(getUser(x), currUser)).ToList();
    }
  }

  public record Query(string UserId, string Predicate) : IRequest<List<Profile>> {
    public bool IsFollowing() => string.Equals(Predicate, "following", StringComparison.InvariantCultureIgnoreCase);
  }
}