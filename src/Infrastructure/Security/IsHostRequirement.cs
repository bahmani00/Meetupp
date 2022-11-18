using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security;

public class IsHostRequirement : IAuthorizationRequirement {
}

public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement> {
  private readonly IHttpContextAccessor httpContextAccessor;
  private readonly DataContext dbContext;

  public IsHostRequirementHandler(IHttpContextAccessor httpContextAccessor, DataContext dbContext) {
    this.dbContext = dbContext;
    this.httpContextAccessor = httpContextAccessor;
  }

  protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement) {
    if (context.User == null || !context.User.Identity.IsAuthenticated) {
      context.Fail();
      return;
    }

    var currUserName =
        context.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

    var activityId = httpContextAccessor.GetId();
    var attendance = await dbContext.UserActivities
      .Include(x => x.AppUser)
      .FirstOrDefaultAsync(x => x.ActivityId == activityId && x.IsHost);

    //var host = activity.UserActivities.FirstOrDefault(x => x.IsHost);

    if (attendance?.AppUser?.UserName == currUserName)
      context.Succeed(requirement);
  }
}