using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    var activity = await dbContext.Activities.FindAsync(activityId);
    var host = activity.UserActivities.FirstOrDefault(x => x.IsHost);

    if (host?.AppUser?.UserName == currUserName)
      context.Succeed(requirement);
  }
}