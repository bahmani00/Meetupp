using System.Security.Claims;
using Application.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Infrastructure.Security;

public class CurrUserService : ICurrUserService {
  public string UserId { get; private set; }

  public CurrUserService(IHttpContextAccessor httpContextAccessor) {
    var httpContext = httpContextAccessor.HttpContext;
    UserId = httpContext?.User?.GetUserId()!;
  }
}

public static class HttpContextAccessorExt {
  public static string? GetUserId(this ClaimsPrincipal user) =>
    user?.FindFirstValue(ClaimTypes.NameIdentifier);

  public static Guid GetId(this IHttpContextAccessor httpContextAccessor) {
    return Guid.Parse(httpContextAccessor.HttpContext!.GetRouteData()!.Values["id"]!.ToString()!);
  }
}