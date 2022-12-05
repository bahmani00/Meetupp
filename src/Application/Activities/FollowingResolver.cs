using Application.Auth;
using AutoMapper;
using Domain;

namespace Application.Activities;

public class FollowingResolver : IValueResolver<UserActivity, AttendeeDto, bool> {
  private readonly ICurrUserService currUserService;

  public FollowingResolver(ICurrUserService currUserService) {
    this.currUserService = currUserService;
  }

  public bool Resolve(UserActivity source, AttendeeDto destination, bool destMember, ResolutionContext context) =>
    currUserService.GetCurrUserAsync().Result.IsFollowing(source.AppUserId);
}