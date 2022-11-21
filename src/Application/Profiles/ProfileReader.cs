using Application.Auth;
using Persistence;
using static Application.Errors.RestException;

namespace Application.Profiles;

public class ProfileReader : IProfileReader {
  private readonly DataContext dbContext;
  private readonly IUserAccessor userAccessor;

  public ProfileReader(DataContext dbContext, IUserAccessor userAccessor) {
    this.dbContext = dbContext;
    this.userAccessor = userAccessor;
  }

  public async Task<Profile> ReadProfileAsync(string username, CancellationToken ct) {
    var user = await dbContext.GetUserProfileAsync(username, ct);
    ThrowIfNotFound(user, new { User = "Not found" });

    var loggedInUser = await userAccessor.GetCurrUserAsync(ct);
    return Profile.From(user, loggedInUser);
  }
}