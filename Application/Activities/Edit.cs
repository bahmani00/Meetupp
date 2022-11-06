using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using AutoMapper;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities;

public class Edit {
  public class Command : IRequest {
    public ActivityDto Activity { get; set; }
  }

  public class CommandValidator : AbstractValidator<Command> {
    public CommandValidator() {
      RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
    }
  }

  public class Handler : IRequestHandler<Command> {
    private readonly DataContext dbContext;
    private readonly IMapper mapper;

    public Handler(DataContext dbContext, IMapper mapper) {
      this.dbContext = dbContext;
      this.mapper = mapper;
    }

    public async Task<Unit> Handle(Command request, CancellationToken ct) {
      var activity = await dbContext.Activities.FindAsync(request.Activity.Id, ct);

      if (activity == null)
        throw new RestException(HttpStatusCode.NotFound, new { Activity = "Not found" });

      mapper.Map(request.Activity, activity);

      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return Unit.Value;

      throw new Exception("Problem saving Activity");
    }
  }
}