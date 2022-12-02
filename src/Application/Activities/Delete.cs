using MediatR;
using Persistence;
using static Application.Errors.RestException;

namespace Application.Activities;

public static class Delete {

  public class Handler : IRequestHandler<Command> {
    private readonly DataContext dbContext;
    public Handler(DataContext dbContext) {
      this.dbContext = dbContext;
    }

    public async Task<Unit> Handle(Command request, CancellationToken ct) {
      var activity = await dbContext.Activities.FindItemAsync(request.Id, ct);
      ThrowIfNotFound(activity, new { Activity = "Not found" });

      dbContext.Remove(activity);

      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return Unit.Value;

      throw new Exception("Problem Deleting activity");
    }
  }

  public record Command(Guid Id) : IRequest;
}