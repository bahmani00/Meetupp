using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Application.Common.Exceptions.RestException;

namespace Application.Profiles;

public static class ListActivities {

  public class Handler : IRequestHandler<Query, List<UserActivityDto>> {
    private readonly IAppDbContext dbContext;
    private readonly IMapper mapper;

    public Handler(IAppDbContext dbContext, IMapper mapper) {
      this.dbContext = dbContext;
      this.mapper = mapper;
    }

    public async Task<List<UserActivityDto>> Handle(Query request, CancellationToken ct) {
      var user = await dbContext.GetUserAsync(request.Username, ct);
      ThrowIfNotFound(user, new { User = "Not found" });

      var queryable = dbContext.UserActivities
        .AsNoTracking()
        .Include(x => x.Activity)
        .Where(x => x.AppUserId == user!.Id!)
        .OrderBy(a => a.Activity.Date)
        .AsQueryable();

      queryable = request.Predicate switch {
        "past" => queryable.Where(a => a.Activity.Date <= DateTime.Now),
        "hosting" => queryable.Where(a => a.IsHost),
        _ => queryable.Where(a => a.Activity.Date >= DateTime.Now),
      };
      await Task.CompletedTask;

      return queryable.ProjectTo<UserActivityDto>(mapper.ConfigurationProvider)
        .OrderBy(t => t.Title)
        .ToList();
    }
  }

  public record Query(string Username, string Predicate) : IRequest<List<UserActivityDto>>;
}