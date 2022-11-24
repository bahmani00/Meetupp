using Application.Auth;
using Persistence;
using static Application.Errors.RestException;

namespace Application.Profiles;

public class ProfileReader : IProfileReader {
  private readonly DataContext dbContext;
  private readonly ICurrUserService currUserService;

  public ProfileReader(DataContext dbContext, ICurrUserService currUserService) {
    this.dbContext = dbContext;
    this.currUserService = currUserService;
  }

  public async Task<Profile> ReadProfileAsync(string username, CancellationToken ct) {
    var user = await dbContext.GetUserProfileAsync(username, ct);
    ThrowIfNotFound(user, new { User = "Not found" });

    var loggedInUser = await currUserService.GetCurrUserAsync(ct);
    return Profile.From(user, loggedInUser);
  }
}