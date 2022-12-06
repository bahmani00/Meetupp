using System.Security.Claims;
using Application.Auth;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Security;

public class CurrUserService : ICurrUserService {
  public string UserId { get; private set; }

  public CurrUserService(IHttpContextAccessor httpContextAccessor) {
    var httpContext = httpContextAccessor.HttpContext;
    UserId = httpContext?.User.GetUserId();
  }

  //public async Task<AppUser> GetCurrUserAsync(CancellationToken ct) {
  //  httpContext.Items["loggedInUser"] ??=
  //    await identityService.GetUserProfileAsync(UserId, ct);
  //  return httpContext.Items["loggedInUser"] as AppUser;
  //}
}

public static class LoggedInUserServiceExt {
  public static string GetUserId(this ClaimsPrincipal user) =>
    user?.FindFirstValue(ClaimTypes.NameIdentifier);
}