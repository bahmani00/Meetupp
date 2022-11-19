using Application.Errors;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities;

public static class Edit {
  public class Command : ActivityDto, IRequest { }

  public class CommandValidator : AbstractValidator<Command> {
    public CommandValidator() {
      RuleFor(x => x).SetValidator(new ActivityValidator());
    }
  }

  public class Handler : IRequestHandler<Command> {
    private readonly DataContext dbContext;

    public Handler(DataContext dbContext) {
      this.dbContext = dbContext;
    }

    public async Task<Unit> Handle(Command request, CancellationToken ct) {
      var activity = await dbContext.Activities.FindItemAsync(request.Id, ct);

      if (activity == null)
        RestException.ThrowNotFound(new { Activity = "Not found" });

      activity = request.ToEntityPartial(activity);

      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return Unit.Value;

      throw new Exception("Problem saving Activity");
    }
  }
}