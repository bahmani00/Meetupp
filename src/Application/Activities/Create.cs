using Application.Common.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;

namespace Application.Activities;

public static class Create {

  internal class Handler : IRequestHandler<Command, Guid> {
    private readonly IAppDbContext dbContext;
    private readonly IIdentityService currUserService;
    private readonly IMapper mapper;

    public Handler(IAppDbContext dbContext, IIdentityService currUserService, IMapper mapper) {
      this.dbContext = dbContext;
      this.currUserService = currUserService;
      this.mapper = mapper;
    }

    public async Task<Guid> Handle(Command request, CancellationToken ct) {
      var activity = mapper.Map<Activity>(request);

      //Dont use AddSync
      dbContext.Activities.Add(activity);

      var userId = currUserService.GetCurrUserId();
      var attendee = UserActivity.Create(userId, activity.Id, true);

      dbContext.UserActivities.Add(attendee);

      var success = await dbContext.SaveChangesAsync(ct) > 0;
      if (success) return activity.Id;

      throw new Exception("Problem Adding changes");
    }
  }

  public class CommandValidator : AbstractValidator<Command> {
    public CommandValidator() {
      RuleFor(x => x).SetValidator(new ActivityValidator());
      RuleFor(x => x.Date).GreaterThan(DateTime.Now);
    }
  }

  /// <summary>
  /// Create Activity model
  /// </summary>
  public class Command : ActivityBaseRequiredDto, IRequest<Guid> { }
}