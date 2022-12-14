using Application.Auth;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity;

public class IdentityService : IIdentityService {
  private readonly ICurrUserService currUserService;
  private readonly UserManager<AppUser> _userManager;
  private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory;
  private readonly IAuthorizationService _authorizationService;

  public IdentityService(
    ICurrUserService currUserService,
    UserManager<AppUser> userManager,
    IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService) {

    this.currUserService = currUserService;
    _userManager = userManager;
    _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    _authorizationService = authorizationService;
  }

  public string GetCurrUserId() => currUserService.UserId;
  private AppUser? currUser;

  public async Task<AppUser> GetCurrUserProfileAsync(CancellationToken ct = default) =>
     currUser ??= await GetUserProfileAsync(currUserService.UserId, ct);
     
  public async Task<AppUser> GetUserProfileAsync(string userId, CancellationToken ct) =>
    await _userManager.Users
      .Include(x => x.Followings)
      .Include(x => x.Followers)
      .Include(x => x.Photos)
      .TagWithCallSite()
      .FirstAsync(u => u.Id == userId, ct);

  public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password) {
    var user = new AppUser {
      UserName = userName,
      Email = userName,
    };

    var result = await _userManager.CreateAsync(user, password);

    return (result.ToApplicationResult(), user.Id);
  }

  public async Task<bool> IsInRoleAsync(string userId, string role) {
    var user = _userManager.Users.SingleOrDefault(x => x.Id == userId);
    return user != null && await _userManager.IsInRoleAsync(user, role);
  }

  public async Task<bool> AuthorizeAsync(string userId, string policyName) {
    var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

    if (user == null) {
      return false;
    }

    var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

    var result = await _authorizationService.AuthorizeAsync(principal, policyName);

    return result.Succeeded;
  }

  public async Task<Result> DeleteUserAsync(string userId) {
    var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

    return user != null ? await DeleteUserAsync(user) : Result.Success();
  }

  public async Task<Result> DeleteUserAsync(AppUser user) {
    var result = await _userManager.DeleteAsync(user);

    return result.ToApplicationResult();
  }
}

public static class IdentityResultExtensions {
  public static Result ToApplicationResult(this IdentityResult result) {
    return result.Succeeded
        ? Result.Success()
        : Result.Failure(result.Errors.Select(e => e.Description));
  }
}