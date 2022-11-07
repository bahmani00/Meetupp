using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Auth;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities;

public class Create {
  public class Command : IRequest<Guid> {
    public ActivityDto Activity { get; set; }
  }

  public class CommandValidator : AbstractValidator<Command> {
    public CommandValidator() {
      RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
      RuleFor(x => x.Activity.Date).GreaterThan(DateTime.Now)
       .WithMessage($"{nameof(Activity.Date)} should be greater than current time");
    }
  }

  public class Handler : IRequestHandler<Command, Guid> {
    private readonly DataContext dbContext;
    private readonly IUserAccessor userAccessor;
    private readonly IMapper mapper;

    public Handler(DataContext dbContext, IUserAccessor userAccessor, IMapper mapper) {
      this.dbContext = dbContext;
      this.userAccessor = userAccessor;
      this.mapper = mapper;
    }

    public async Task<Guid> Handle(Command request, CancellationToken ct) {
      var activity = new Activity();
      mapper.Map(request.Activity, activity);

      //Dont use AddSync
      dbContext.Activities.Add(activity);

      var user = await dbContext.Users.SingleOrDefaultAsync(x =>
          x.UserName == userAccessor.GetCurrentUsername(), ct);

      var attendee = new UserActivity {
          AppUser = user,
          Activity = activity,
          IsHost = true,
          DateJoined = DateTime.Now
      };

      dbContext.UserActivities.Add(attendee);

      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return activity.Id;

      throw new Exception("Problem Adding changes");
    }
  }
}