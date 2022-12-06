using Application.Common.Interfaces;
using AutoMapper;
using Domain;

namespace Application.Activities;

public class FollowingResolver : IValueResolver<UserActivity, AttendeeDto, bool> {
  private readonly IIdentityService currUserService;

  public FollowingResolver(IIdentityService currUserService) {
    this.currUserService = currUserService;
  }

  public bool Resolve(UserActivity source, AttendeeDto destination, bool destMember, ResolutionContext context) =>
    currUserService.GetCurrUserProfileAsync().Result.IsFollowing(source.AppUserId);
}