using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities;

public static class EditPartial {
  public class Command : ActivityDto, IRequest {
  }

  public class CommandValidator : AbstractValidator<Command> {
    public CommandValidator() {
      RuleFor(x => x.Id).NotEmpty();
      RuleFor(command => command)
        .Must(x => !string.IsNullOrEmpty(x.Title) |
            !string.IsNullOrEmpty(x.Description) ||
            !string.IsNullOrEmpty(x.Category) ||
            !x.Date.HasValue ||
            !string.IsNullOrEmpty(x.City) ||
            !string.IsNullOrEmpty(x.Venue)
          )
          .WithMessage($"Provide at least either {nameof(Activity.Title)}, {nameof(Activity.Description)}, {nameof(Activity.Category)}, {nameof(Activity.Date)}, {nameof(Activity.City)} or {nameof(Activity.Venue)}");
      RuleFor(command => command)
        .Must(x => !x.Date.HasValue || (x.Date >= DateTime.Now))
        .WithMessage($"{nameof(Activity.Date)} should be greater than current time");
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

      throw new Exception($"Problem saving {nameof(Activity)}");
    }
  }
}