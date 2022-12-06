using Application.Common.Interfaces;
using static Application.Errors.RestException;

namespace Application.Profiles;

public class ProfileReader : IProfileReader {
  private readonly IAppDbContext dbContext;
  private readonly IIdentityService currUserService;

  public ProfileReader(IAppDbContext dbContext, IIdentityService currUserService) {
    this.dbContext = dbContext;
    this.currUserService = currUserService;
  }

  public async Task<Profile> ReadProfileAsync(string username, CancellationToken ct) {
    var user = await dbContext.GetUserProfileAsync(username, ct);
    ThrowIfNotFound(user, new { User = "Not found" });

    var loggedInUser = await currUserService.GetCurrUserProfileAsync(ct);
    return Profile.From(user, loggedInUser);
  }
}