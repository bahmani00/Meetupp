using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities;

public static class Details {
  public class Query : IRequest<ActivityDto> {
    public Guid Id { get; set; }
  }

  public class Handler : IRequestHandler<Query, ActivityDto> {
    private readonly DataContext dbContext;
    private readonly IMapper mapper;

    public Handler(DataContext dbContext, IMapper mapper) {
      this.dbContext = dbContext;
      this.mapper = mapper;
    }

    public async Task<ActivityDto> Handle(Query request, CancellationToken ct) {
      var activity = await dbContext.Activities.FindItemAsync(request.Id, ct);

      if (activity == null)
        RestException.ThrowNotFound(new { Activity = "Not found" });

      var activityToReturn = mapper.Map<Activity, ActivityDto>(activity);

      return activityToReturn;
    }
  }
}