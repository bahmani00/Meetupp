using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using static Application.Errors.RestException;

namespace Application.Activities;

public static class Details {

  public class Handler : IRequestHandler<Query, ActivityDto> {
    private readonly DataContext dbContext;
    private readonly IMapper mapper;

    public Handler(DataContext dbContext, IMapper mapper) {
      this.dbContext = dbContext;
      this.mapper = mapper;
    }

    public async Task<ActivityDto> Handle(Query request, CancellationToken ct) {
      var activity = await dbContext.Activities
        .Include(x => x.Comments).ThenInclude(x => x.Author).ThenInclude(x => x.Photos)
        .Include(x => x.UserActivities).ThenInclude(x => x.AppUser).ThenInclude(x => x.Photos)
        .SingleOrDefaultAsync(x => x.Id == request.Id, ct);

      ThrowIfNotFound(activity, new { Activity = "Not found" });

      return mapper.Map<ActivityDto>(activity);
    }
  }

  public record Query(Guid Id): IRequest<ActivityDto>;
}