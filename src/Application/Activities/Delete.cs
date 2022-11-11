using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using MediatR;
using Persistence;

namespace Application.Activities;

public static class Delete {
  public class Command : IRequest {
    public Guid Id { get; set; }
  }

  public class Handler : IRequestHandler<Command> {
    private readonly DataContext dbContext;
    public Handler(DataContext dbContext) {
      this.dbContext = dbContext;
    }

    public async Task<Unit> Handle(Command request, CancellationToken ct) {
      var activity = await dbContext.Activities.FindItemAsync(request.Id, ct);

      if (activity == null)
        throw new RestException(HttpStatusCode.NotFound, new { Activity = "Not found" });

      dbContext.Remove(activity);

      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return Unit.Value;

      throw new Exception("Problem Deleting activity");
    }
  }
}