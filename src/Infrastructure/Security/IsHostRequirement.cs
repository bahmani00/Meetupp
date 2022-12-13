using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Security;

public class IsHostRequirement : IAuthorizationRequirement {
  public const string PolicyName = "IsHostCreatedActivity";
}

public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement> {
  private readonly IHttpContextAccessor httpContextAccessor;
  private readonly IAppDbContext dbContext;

  public IsHostRequirementHandler(IHttpContextAccessor httpContextAccessor, IAppDbContext dbContext) {
    this.dbContext = dbContext;
    this.httpContextAccessor = httpContextAccessor;
  }

  protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement) {
    if (context.User == null || !context.User.Identity!.IsAuthenticated) {
      context.Fail();
      return;
    }

    var userId = context.User.GetUserId();
    var activityId = httpContextAccessor.GetId();

    var attendance = await dbContext.UserActivities
      .Include(x => x.AppUser)
      .FirstOrDefaultAsync(x => x.ActivityId == activityId && x.IsHost && x.AppUser.Id == userId);

    if (attendance != null)
      context.Succeed(requirement);
  }
}