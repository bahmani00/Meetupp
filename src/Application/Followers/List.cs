using Application.Common.Interfaces;
using Application.Profiles;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Followers;

public static class List {

  public class Handler : IRequestHandler<Query, List<Profile>> {
    private readonly IAppDbContext dbContext;
    private readonly IProfileReader profileReader;

    public Handler(IAppDbContext dbContext, IProfileReader profileReader) {
      this.dbContext = dbContext;
      this.profileReader = profileReader;
    }

    public async Task<List<Profile>> Handle(Query request, CancellationToken ct) {
      var queryable = dbContext.Followings
        .AsNoTracking()
        .Include(x => x.Observer)
        .Include(x => x.Target)
        .AsQueryable();

      var profiles = new List<Profile>();

      switch (request.Predicate) {
        case "followers": {
            var userFollowings = await queryable.Where(x =>
                x.Target.Id == request.UserId).ToListAsync(ct);

            foreach (var follower in userFollowings) {
              profiles.Add(await profileReader.ReadProfileAsync(follower.Observer.UserName, ct));
            }
            break;
          }
        case "following": {
            var userFollowings = await queryable.Where(x =>
                x.Observer.Id == request.UserId).ToListAsync(ct);

            foreach (var follower in userFollowings) {
              profiles.Add(await profileReader.ReadProfileAsync(follower.Target.UserName, ct));
            }
            break;
          }
      }

      return profiles;
    }
  }

  public record Query(string UserId, string Predicate) : IRequest<List<Profile>>;

}