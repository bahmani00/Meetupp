using Domain;

namespace Application.Auth;

public interface IUserAccessor {

  string GetCurrUsername();

  Task<AppUser> GetCurrUserAsync(CancellationToken ct = default);
}