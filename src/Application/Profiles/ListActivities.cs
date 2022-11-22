using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using static Application.Errors.RestException;

namespace Application.Profiles;

public static class ListActivities {
  public class Query : IRequest<List<UserActivityDto>> {
    public string Username { get; set; }
    public string Predicate { get; set; }
  }

  public class Handler : IRequestHandler<Query, List<UserActivityDto>> {
    private readonly DataContext dbContext;
    private readonly IMapper mapper;

    public Handler(DataContext dbContext, IMapper mapper) {
      this.dbContext = dbContext;
      this.mapper = mapper;
    }

    public async Task<List<UserActivityDto>> Handle(Query request, CancellationToken ct) {
      var user = await dbContext.GetUserAsync(request.Username, ct);
      ThrowIfNotFound(user, new { User = "Not found" });

      var queryable = dbContext.UserActivities
        .AsNoTracking()
        .Include(x => x.Activity)
        .Where(x => x.AppUserId == user.Id)
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
}