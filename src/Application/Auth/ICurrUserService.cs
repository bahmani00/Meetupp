using Domain;

namespace Application.Auth;

public interface ICurrUserService {

  string UserId { get; }

  Task<AppUser> GetCurrUserAsync(CancellationToken ct = default);
}