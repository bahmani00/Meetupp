using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Auth;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Activities;

public static class List {
  public class ActivitiesEnvelope {
    public List<ActivityDto> Activities { get; set; }
    public int ActivityCount { get; set; }
  }

  public class Query : IRequest<ActivitiesEnvelope> {
    public Query(int? limit, int? offset, bool isGoing, bool isHost, DateTime? startDate) {
      Limit = limit;
      Offset = offset;
      IsGoing = isGoing;
      IsHost = isHost;
      StartDate = startDate ?? DateTime.Now;
    }

    public bool IsGoing { get; set; }
    public bool IsHost { get; set; }
    public DateTime? StartDate { get; set; }

    public int? Limit { get; set; }
    public int? Offset { get; set; }
  }

  public class Handler : IRequestHandler<Query, ActivitiesEnvelope> {
    private readonly DataContext dbContext;
    private readonly ILogger<Handler> logger;
    private readonly IMapper mapper;
    private readonly IUserAccessor userAccessor;

    public Handler(DataContext dbContext, IMapper mapper, IUserAccessor userAccessor, ILogger<Handler> logger) {
      this.dbContext = dbContext;
      this.userAccessor = userAccessor;
      this.mapper = mapper;
      this.logger = logger;
    }

    public async Task<ActivitiesEnvelope> Handle(Query request, CancellationToken ct) {
      var queryable = dbContext.Activities
        .AsNoTracking()
        .Include(x => x.Comments).ThenInclude(x => x.Author)
        .Include(x => x.UserActivities).ThenInclude(x => x.AppUser).ThenInclude(x => x.Photos)
        .Where(x => x.Date >= request.StartDate)
        .OrderBy(x => x.Date)
        .AsQueryable();

      if (request.IsGoing && !request.IsHost) {
        queryable = queryable.Where(x => x.UserActivities.Any(a => a.AppUser.UserName == userAccessor.GetCurrentUsername()));
      }

      if (request.IsHost && !request.IsGoing) {
        queryable = queryable.Where(x => x.UserActivities.Any(a => a.AppUser.UserName == userAccessor.GetCurrentUsername() && a.IsHost));
      }

      var activities = await queryable
          .Skip(request.Offset ?? 0)
          .Take(request.Limit ?? 3).ToListAsync(ct);

      return new ActivitiesEnvelope {
        Activities = mapper.Map<List<ActivityDto>>(activities),
        ActivityCount = queryable.Count()
      };
    }

    public async Task<List<ActivityDto>> Handle111(Query request, CancellationToken ct) {
      // try {
      //   request.ToString();

      //   for (var i = 0; i < 5; i++) {
      //     ct.ThrowIfCancellationRequested();
      //     await Task.Delay(1000, ct);
      //     logger.LogInformation($"Task {i} has completed");
      //   }
      // } catch (Exception ex) when (ex is TaskCanceledException) {
      //   logger.LogInformation("Task was cancelled.");
      // }
      await Task.CompletedTask;

      return mapper.Map<List<Activity>, List<ActivityDto>>(null);
    }
  }
}