using Application.Auth;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities;

public static class Create {

  internal class Handler : IRequestHandler<Command, Guid> {
    private readonly DataContext dbContext;
    private readonly ICurrUserService currUserService;

    public Handler(DataContext dbContext, ICurrUserService currUserService) {
      this.dbContext = dbContext;
      this.currUserService = currUserService;
    }

    public async Task<Guid> Handle(Command request, CancellationToken ct) {
      var activity = request.ToEntity();

      //Dont use AddSync
      dbContext.Activities.Add(activity);

      var user = await currUserService.GetCurrUserAsync();
      var attendee = UserActivity.Create(user, activity, true);

      dbContext.UserActivities.Add(attendee);

      var success = await dbContext.SaveChangesAsync(ct) > 0;
      if (success) return activity.Id;

      throw new Exception("Problem Adding changes");
    }
  }
  
  public class Command : ActivityDto, IRequest<Guid> {
  }

  public class CommandValidator : AbstractValidator<Command> {
    public CommandValidator() {
      RuleFor(x => x).SetValidator(new ActivityValidator());
      RuleFor(x => x.Date).GreaterThan(DateTime.Now);
    }
  }

}