using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities;

public static class EditPartial {
  public class Command : IRequest {
    public ActivityDto Activity { get; set; }
  }

  public class CommandValidator : AbstractValidator<Command> {
    public CommandValidator() {
      RuleFor(x => x.Activity.Id).NotEmpty();
      RuleFor(command => command)
          .Must(x => !string.IsNullOrEmpty(x.Activity.Title) |
                     !string.IsNullOrEmpty(x.Activity.Description) ||
                     !string.IsNullOrEmpty(x.Activity.Category) ||
                     !x.Activity.Date.HasValue ||
                     !string.IsNullOrEmpty(x.Activity.City) ||
                     !string.IsNullOrEmpty(x.Activity.Venue)
              )
          .WithMessage($"Provide at least either {nameof(Activity.Title)}, {nameof(Activity.Description)}, {nameof(Activity.Category)}, {nameof(Activity.Date)}, {nameof(Activity.City)} or {nameof(Activity.Venue)}");
      RuleFor(command => command)
          .Must(x => !x.Activity.Date.HasValue || (x.Activity.Date >= DateTime.Now))
          .WithMessage($"{nameof(Activity.Date)} should be greater than current time");
    }
  }

  public class Handler : IRequestHandler<Command> {
    private readonly DataContext dbContext;

    public Handler(DataContext dbContext) {
      this.dbContext = dbContext;
    }

    public async Task<Unit> Handle(Command request, CancellationToken ct) {
      var activity = await dbContext.Activities.FindItemAsync(request.Activity.Id, ct);

      if (activity == null)
        throw new RestException(HttpStatusCode.NotFound, new { Activity = "Not found" });

      activity.Title = request.Activity.Title ?? activity.Title;
      activity.Description = request.Activity.Description ?? activity.Description;
      activity.Category = request.Activity.Category ?? activity.Category;
      activity.Date = request.Activity.Date ?? activity.Date;
      activity.City = request.Activity.City ?? activity.City;
      activity.Venue = request.Activity.Venue ?? activity.Venue;

      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return Unit.Value;

      throw new Exception($"Problem saving {nameof(Activity)}");
    }
  }
}