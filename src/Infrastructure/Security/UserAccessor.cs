using System.Security.Claims;
using Application.Auth;
using Domain;
using Microsoft.AspNetCore.Http;
using Persistence;

namespace Infrastructure.Security;

public class UserAccessor : IUserAccessor {
  private readonly DataContext dbContext;
  private readonly HttpContext httpContext;

  public UserAccessor(DataContext dbContext, IHttpContextAccessor httpContextAccessor) {
    this.dbContext = dbContext;
    this.httpContext = httpContextAccessor.HttpContext;
  }

  public string GetCurrentUsername() {
    return httpContext.User?.Claims?
      .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
  }

  public async Task<AppUser> GetCurrentUserAsync(CancellationToken ct) {
    httpContext.Items["loggedInUser"] ??=
      await dbContext.GetUserAsync(GetCurrentUsername(), false, ct);

    return httpContext.Items["loggedInUser"] as AppUser;
  }
}