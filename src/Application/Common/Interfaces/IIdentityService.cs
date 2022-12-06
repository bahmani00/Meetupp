using Application.Common.Models;
using Domain;

namespace Application.Common.Interfaces;

public interface IIdentityService {
  string GetCurrUserId();

  Task<AppUser> GetCurrUserProfileAsync(CancellationToken ct = default);

  Task<AppUser> GetUserProfileAsync(string userId, CancellationToken ct);

  Task<bool> IsInRoleAsync(string userId, string role);

  Task<bool> AuthorizeAsync(string userId, string policyName);

  Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

  Task<Result> DeleteUserAsync(string userId);
}