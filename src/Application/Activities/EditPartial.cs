using Application.Common.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using static Application.Common.Exceptions.RestException;

namespace Application.Activities;

public static class EditPartial {

  public class Handler : IRequestHandler<Command> {
    private readonly IAppDbContext dbContext;
    private readonly IMapper mapper;

    public Handler(IAppDbContext dbContext, IMapper mapper) {
      this.dbContext = dbContext;
      this.mapper = mapper;
    }

    public async Task<Unit> Handle(Command request, CancellationToken ct) {
      var activity = await dbContext.Activities.FindItemAsync(request.Id, ct);
      ThrowIfNotFound(activity, new { Activity = "Not found" });

      mapper.Map(request, activity);

      var success = await dbContext.SaveChangesAsync(ct) > 0;
      if (success) return Unit.Value;

      throw new Exception($"Problem saving {nameof(Activity)}");
    }
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

  public record Command(
    Guid Id,
    string Title,
    string Description,
    string Category,
    DateTime? Date,
    string City,
    string Venue) : IRequest;
}