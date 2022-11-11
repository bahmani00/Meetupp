using System.Linq;
using Application.Auth;
using AutoMapper;
using Domain;
using Persistence;

namespace Application.Activities;

public class FollowingResolver : IValueResolver<UserActivity, AttendeeDto, bool> {
  private readonly DataContext dbContext;
  private readonly IUserAccessor userAccessor;

  public FollowingResolver(DataContext dbContext, IUserAccessor userAccessor) {
    this.dbContext = dbContext;
    this.userAccessor = userAccessor;
  }

  public bool Resolve(UserActivity source, AttendeeDto destination, bool destMember, ResolutionContext context) {
    var currentUser = dbContext.Users.SingleOrDefault(x => x.UserName == userAccessor.GetCurrentUsername());

    if (currentUser.Followings.Any(x => x.TargetId == source.AppUserId))
      return true;

    return false;
  }
}