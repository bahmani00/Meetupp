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
using Persistence;
using Microsoft.Extensions.Logging;

namespace Application.Activities;

public class List {
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
    private readonly DataContext _context;
    private readonly ILogger<List> _logger;
    private readonly IMapper _mapper;
    private readonly IUserAccessor _userAccessor;

    public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor, ILogger<List> logger) {
      _userAccessor = userAccessor;
      _mapper = mapper;
      _context = context;
      _logger = logger;
    }

    public async Task<ActivitiesEnvelope> Handle(Query request, CancellationToken ct) {
      var queryable = _context.Activities
          .Where(x => x.Date >= request.StartDate)
          .OrderBy(x => x.Date)
          .AsQueryable();

      if (request.IsGoing && !request.IsHost) {
        queryable = queryable.Where(x => x.UserActivities.Any(a => a.AppUser.UserName == _userAccessor.GetCurrentUsername()));
      }

      if (request.IsHost && !request.IsGoing) {
        queryable = queryable.Where(x => x.UserActivities.Any(a => a.AppUser.UserName == _userAccessor.GetCurrentUsername() && a.IsHost));
      }

      var activities = await queryable
          .Skip(request.Offset ?? 0)
          .Take(request.Limit ?? 3).ToListAsync();

      return new ActivitiesEnvelope {
        Activities = _mapper.Map<List<Activity>, List<ActivityDto>>(activities),
        ActivityCount = queryable.Count()
      };
    }

    public async Task<List<ActivityDto>> Handle111(Query request, CancellationToken ct) {
      // try
      // {
      //     for(var i = 0; i < 5; i++){
      //         ct.ThrowIfCancellationRequested();
      //         await Task.Delay(1000, ct);
      //         _logger.LogInformation($"Task {i} has completed");
      //     }
      // }
      // catch (Exception ex) when(ex is TaskCanceledException)
      // {
      //     _logger.LogInformation("Task was cancelled.");
      // }

      var activities = await _context.Activities
          .ToListAsync();

      return _mapper.Map<List<Activity>, List<ActivityDto>>(activities);
    }
  }
}