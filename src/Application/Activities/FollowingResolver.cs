using System.Linq;
using Application.Auth;
using AutoMapper;
using Domain;

namespace Application.Activities;

public class FollowingResolver : IValueResolver<UserActivity, AttendeeDto, bool> {
  private readonly IUserAccessor userAccessor;

  public FollowingResolver(IUserAccessor userAccessor) {
    this.userAccessor = userAccessor;
  }

  public bool Resolve(UserActivity source, AttendeeDto destination, bool destMember, ResolutionContext context) =>
    userAccessor.GetCurrentUserAsync().Result.Followings.Any(x => x.TargetId == source.AppUserId);
}