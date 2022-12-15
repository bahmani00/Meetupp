using Application.Common.Interfaces;
using AutoMapper;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Application.Common.Exceptions.RestException;

namespace Application.Activities;

public static class Details {

  internal class Handler : IRequestHandler<Query, ActivityDetailDto> {
    private readonly IAppDbContext dbContext;
    private readonly IMapper mapper;

    public Handler(IAppDbContext dbContext, IMapper mapper) {
      this.dbContext = dbContext;
      this.mapper = mapper;
    }

    public async Task<ActivityDetailDto> Handle(Query request, CancellationToken ct) {
      var entity = await dbContext.Activities
        .AsNoTracking()
        .Include(x => x.Comments.Where(y => y.ActivityId == request.Id).OrderBy(c => c.CreatedOn))
          .ThenInclude(x => x.CreatedBy).ThenInclude(x => x.Photos)
        .Include(x => x.UserActivities.Where(y => y.ActivityId == request.Id))
          .ThenInclude(x => x.AppUser).ThenInclude(x => x.Photos)
        .AsSplitQuery()
        //.ProjectTo<ActivityDetailDto>(mapper.ConfigurationProvider)
        .TagWithCallSite()
        .SingleOrDefaultAsync(x => x.Id == request.Id, ct);
      
      ThrowIfNotFound(entity, new { Activity = "Not found" });

      return mapper.Map<ActivityDetailDto>(entity);
    }
  }

  public record Query(Guid Id) : IRequest<ActivityDetailDto>;
}