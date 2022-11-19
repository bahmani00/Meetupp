using Domain;

namespace Application.Auth;

public interface IUserAccessor {

  string GetCurrentUsername();

  Task<AppUser> GetCurrentUserAsync(CancellationToken ct = default);
}