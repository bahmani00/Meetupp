using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Profiles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers;

public class List {
  public class Query : IRequest<List<Profile>> {
    public string Username { get; set; }
    public string Predicate { get; set; }
  }

  public class Handler : IRequestHandler<Query, List<Profile>> {
    private readonly DataContext dbContext;
    private readonly IProfileReader profileReader;

    public Handler(DataContext dbContext, IProfileReader profileReader) {
      this.dbContext = dbContext;
      this.profileReader = profileReader;
    }

    public async Task<List<Profile>> Handle(Query request, CancellationToken ct) {
      var queryable = dbContext.Followings.AsQueryable();

      var profiles = new List<Profile>();

      switch (request.Predicate) {
        case "followers": {
            var userFollowings = await queryable.Where(x =>
                x.Target.UserName == request.Username).ToListAsync(ct);

            foreach (var follower in userFollowings) {
              profiles.Add(await profileReader.ReadProfileAsync(follower.Observer.UserName, ct));
            }
            break;
          }
        case "following": {
            var userFollowings = await queryable.Where(x =>
                x.Observer.UserName == request.Username).ToListAsync(ct);

            foreach (var follower in userFollowings) {
              profiles.Add(await profileReader.ReadProfileAsync(follower.Target.UserName, ct));
            }
            break;
          }
      }

      return profiles;
    }
  }
}