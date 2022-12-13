using Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Application.Common.Exceptions.RestException;

namespace Application.Activities;

public static class Details {

  public class Handler : IRequestHandler<Query, ActivityDto> {
    private readonly IAppDbContext dbContext;
    private readonly IMapper mapper;

    public Handler(IAppDbContext dbContext, IMapper mapper) {
      this.dbContext = dbContext;
      this.mapper = mapper;
    }

    public async Task<ActivityDto> Handle(Query request, CancellationToken ct) {
      var entity = await dbContext.Activities
        .AsNoTracking()
        .Include(x => x.Comments.Where(y => y.ActivityId == request.Id))
          .ThenInclude(x => x.CreatedBy).ThenInclude(x => x.Photos)
        .Include(x => x.UserActivities.Where(y => y.ActivityId == request.Id))
          .ThenInclude(x => x.AppUser).ThenInclude(x => x.Photos)
        .AsSplitQuery()
        .SingleOrDefaultAsync(x => x.Id == request.Id, ct);
      //var entity = await (
      //                    from activity in dbContext.Activities.AsNoTracking()
      //                    join comment in dbContext.Comments.AsNoTracking()
      //                        on activity.Id equals comment.Id into comments
      //                    join userActivity in dbContext.UserActivities.AsNoTracking()
      //                        on activity.Id equals userActivity.ActivityId into userActivities
      //                    join user in dbContext.Users.AsNoTracking()
      //                        on activity.CreatedById equals user.Id into users
      //                    where activity.Id == request.Id
      //                    select activity).SingleOrDefaultAsync(ct);

      ThrowIfNotFound(entity, new { Activity = "Not found" });

      return mapper.Map<ActivityDto>(entity);
    }
  }

  public record Query(Guid Id) : IRequest<ActivityDto>;
}