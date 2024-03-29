using AutoMapper;
using Domain;

namespace Application.Activities;

public class MappingProfile : Profile {
  public MappingProfile() {
    CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);
    CreateMap<DateTimeOffset, DateTime>().ConvertUsing((src, dest) => src.UtcDateTime);

    //https://stackoverflow.com/a/68012002/336511
    CreateMap<EditPartial.Command, Activity>()
      .ForAllMembers(opts =>
        opts.Condition((src, dest, srcMember) => srcMember != null));

    CreateMap<Edit.Command, Activity>();

    CreateMap<Create.Command, Activity>();

    CreateMap<Activity, ActivityDetailDto>();
    CreateMap<Activity, ActivityDto>();

    //CreateMap<ActivityDto, Activity>(MemberList.None);

    CreateMap<UserActivity, Profiles.UserActivityDto>()
        .ForMember(d => d.Id, o => o.MapFrom(s => s.Activity.Id))
        .ForMember(d => d.Title, o => o.MapFrom(s => s.Activity.Title))
        .ForMember(d => d.Category, o => o.MapFrom(s => s.Activity.Category))
        .ForMember(d => d.Date, o => o.MapFrom(s => s.Activity.Date));

    AppUser? currUser = default;
    CreateMap<UserActivity, AttendeeDto>()
        .ForMember(d => d.UserId, o => o.MapFrom(s => s.AppUserId))
        .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
        .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
        .ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.MainPhotoUrl))
        //Value Resolvers aren't supported for queryable extensions.
        //https://stackoverflow.com/a/27567113/336511
        //.ForMember(d => d.Following, o => o.MapFrom<FollowingResolver>())
        .ForMember(d => d.Following, o => o.MapFrom(src => currUser!.IsFollowing(src.AppUserId)));
  }
}