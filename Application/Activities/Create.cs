using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Auth;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities;

public class Create {
    public class Command : IRequest<Guid> {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command> {
        public CommandValidator() {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Category).NotEmpty();
            RuleFor(x => x.Date).NotEmpty().GreaterThan(DateTime.Now)
                .WithMessage("Date should be greater than current time");
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.Venue).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Command, Guid> {
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;
        public Handler(DataContext context, IUserAccessor userAccessor) {
            _userAccessor = userAccessor;
            _context = context;
        }

        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken) {
            var activity = new Activity {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Category = request.Category,
                Date = request.Date,
                City = request.City,
                Venue = request.Venue
            };

            //Dont use AddSync
            _context.Activities.Add(activity);

            var user = await _context.Users.SingleOrDefaultAsync(x =>
                x.UserName == _userAccessor.GetCurrentUsername());

            var attendee = new UserActivity {
                AppUser = user,
                Activity = activity,
                IsHost = true,
                DateJoined = DateTime.Now
            };

            _context.UserActivities.Add(attendee);

            var success = await _context.SaveChangesAsync() > 0;

            if (success) return activity.Id;

            throw new Exception("Problem Adding changes");
        }
    }
}