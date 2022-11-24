using System.Security.Claims;
using Application.Auth;
using Domain;
using Microsoft.AspNetCore.Http;
using Persistence;

namespace Infrastructure.Security;

public class CurrUserService : ICurrUserService {
  private readonly DataContext dbContext;
  private readonly HttpContext httpContext;

  public string UserId => httpContext.User.GetUsername();

  public CurrUserService(DataContext dbContext, IHttpContextAccessor httpContextAccessor) {
    this.dbContext = dbContext;
    this.httpContext = httpContextAccessor.HttpContext;
  }

  public async Task<AppUser> GetCurrUserAsync(CancellationToken ct) {
    httpContext.Items["loggedInUser"] ??=
      await dbContext.GetUserProfileAsync(UserId, ct);

    return httpContext.Items["loggedInUser"] as AppUser;
  }
}

public static class LoggedInUserServiceExt {
  public static string GetUsername(this ClaimsPrincipal user) =>
    user?.FindFirstValue(ClaimTypes.NameIdentifier);
}