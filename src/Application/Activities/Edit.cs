using AutoMapper;
using FluentValidation;
using MediatR;
using Persistence;
using static Application.Errors.RestException;

namespace Application.Activities;

public static class Edit {

  public class Handler : IRequestHandler<Command> {
    private readonly DataContext dbContext;
    private readonly IMapper mapper;

    public Handler(DataContext dbContext, IMapper mapper) {
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
    }
  }

  public class Command: ActivityBaseDto, IRequest {
    internal Guid Id { get; set; }

    public Command SetId(Guid id) {
      Id = id;
      return this;
    }
  }
}