using AutoMapper;
using Domain;

namespace Application.Activities;

public class MappingProfile : Profile {
  public MappingProfile() {
    CreateMap<Activity, ActivityDto>();

    //CreateMap<ActivityDto, Activity>(MemberList.None);

    CreateMap<UserActivity, Profiles.UserActivityDto>()
        .ForMember(d => d.Id, o => o.MapFrom(s => s.Activity.Id))
        .ForMember(d => d.Title, o => o.MapFrom(s => s.Activity.Title))
        .ForMember(d => d.Category, o => o.MapFrom(s => s.Activity.Category))
        .ForMember(d => d.Date, o => o.MapFrom(s => s.Activity.Date));

    AppUser currUser = default;
    CreateMap<UserActivity, AttendeeDto>()
        .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
        .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
        .ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.MainPhotoUrl))
        //Value Resolvers aren't supported for queryable extensions.
        //https://stackoverflow.com/a/27567113/336511
        //.ForMember(d => d.Following, o => o.MapFrom<FollowingResolver>())
        .ForMember(d => d.Following, o => o.MapFrom(src => currUser.IsFollowing(src.AppUserId)));
  }
}