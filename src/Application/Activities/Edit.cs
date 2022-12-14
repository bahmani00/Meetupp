using System.Text.Json.Serialization;
using Application.Common.Interfaces;
using AutoMapper;
using FluentValidation;
using MediatR;
using static Application.Common.Exceptions.RestException;

namespace Application.Activities;

public static class Edit {

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

      throw new Exception("Problem saving Activity");
    }
  }

  public class CommandValidator : AbstractValidator<Command> {
    public CommandValidator() {
      RuleFor(x => x).SetValidator(new ActivityValidator());
      RuleFor(x => x.Id).NotEmpty();
    }
  }

  /// <summary>
  /// EditActivity model
  /// </summary>
  public class Command : ActivityBaseRequiredDto, IRequest {
    [JsonIgnore]
    public Guid Id { get; set; }
  }
}