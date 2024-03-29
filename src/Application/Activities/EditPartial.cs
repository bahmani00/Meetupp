using System.Text.Json.Serialization;
using Application.Common.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using static Application.Common.Exceptions.RestException;

namespace Application.Activities;

public static class EditPartial {

  internal class Handler : IRequestHandler<Command, ActivityDetailDto> {
    private readonly IAppDbContext dbContext;
    private readonly IMapper mapper;

    public Handler(IAppDbContext dbContext, IMapper mapper) {
      this.dbContext = dbContext;
      this.mapper = mapper;
    }

    public async Task<ActivityDetailDto> Handle(Command request, CancellationToken ct) {
      var activity = await dbContext.Activities.FindItemAsync(request.Id, ct);
      ThrowIfNotFound(activity, new { Activity = "Not found" });

      mapper.Map(request, activity);

      var success = await dbContext.SaveChangesAsync(ct) > 0;
      if (success) return mapper.Map<ActivityDetailDto>(activity);

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
            !x.DateHasValue() ||
            !string.IsNullOrEmpty(x.City) ||
            !string.IsNullOrEmpty(x.Venue)
          )
        .WithMessage($"Provide at least either {nameof(Activity.Title)}, {nameof(Activity.Description)}, {nameof(Activity.Category)}, {nameof(Activity.Date)}, {nameof(Activity.City)} or {nameof(Activity.Venue)}");
    }
  }

  /// <summary>
  /// EditPartial model
  /// </summary>
  public class Command : ActivityBaseDto, IRequest<ActivityDetailDto> {
    [JsonIgnore]
    public Guid Id { get; set; }
  }
}