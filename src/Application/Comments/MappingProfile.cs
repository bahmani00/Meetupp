using AutoMapper;
using Domain;

namespace Application.Comments;

public class MappingProfile : Profile {
  public MappingProfile() {
    CreateMap<Comment, CommentDto>()
        .ForMember(d => d.Username, o => o.MapFrom(s => s.CreatedBy.UserName))
        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn.UtcDateTime))
        .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.CreatedBy.DisplayName))
        .ForMember(d => d.Image, o => o.MapFrom(s => s.CreatedBy.MainPhotoUrl));
  }
}