using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.Auth;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Threading;

namespace Application.Profiles;

public class ProfileReader : IProfileReader {
  private readonly DataContext dbContext;
  private readonly IUserAccessor userAccessor;
  
  public ProfileReader(DataContext dbContext, IUserAccessor userAccessor) {
    this.dbContext = dbContext;
    this.userAccessor = userAccessor;
  }

  public async Task<Profile> ReadProfileAsync(string username, CancellationToken ct) {
    var user = await dbContext.Users.SingleOrDefaultAsync(x => x.UserName == username, ct);

    if (user == null)
      throw new RestException(HttpStatusCode.NotFound, new { User = "Not found" });

    var loggedInUser = await dbContext.Users
      .SingleOrDefaultAsync(x => x.UserName == userAccessor.GetCurrentUsername(), ct);

    var profile = new Profile {
      DisplayName = user.DisplayName,
      Username = user.UserName,
      Image = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
      Photos = user.Photos,
      Bio = user.Bio,
      FollowersCount = user.Followers.Count,
      FollowingCount = user.Followings.Count,
    };

    if (loggedInUser.Followings.Any(x => x.TargetId == user.Id)) {
      profile.IsFollowed = true;
    }

    return profile;
  }
}