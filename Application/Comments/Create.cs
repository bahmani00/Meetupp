using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments;
public class Create {
    public class Command : IRequest<CommentDto> {
        public string Body { get; set; }
        public Guid ActivityId { get; set; }
        public string Username { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command> {
        private readonly DataContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CommandValidator(DataContext dbContext, IHttpContextAccessor httpContextAccessor) {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;

            RuleFor(x => x.ActivityId).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("ActivityId can not be null")
                .NotEmpty().WithMessage("ActivityId can not be empty")
                .Must(ExistsInDatabase).WithMessage("The activity does not exists");
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Body).NotEmpty();
        }

        private bool ExistsInDatabase(Guid activityId) {
            var activity = _dbContext.Activities.Find(activityId);

            if (activity == null)
                return false;

            _httpContextAccessor.HttpContext.Items[$"Activity_{activityId}"] = activity;


            return true;
        }
    }

    public class Handler : IRequestHandler<Command, CommentDto> {
        private readonly DataContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Handler(DataContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor) {
            _mapper = mapper;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommentDto> Handle(Command request, CancellationToken cancellationToken) {
            // var activity = await _dbContext.Activities.FindAsync(request.ActivityId);
            // if (activity == null)
            //     throw new RestException(HttpStatusCode.NotFound, new {Activity = "Not found"});
            var activity = (Activity)_httpContextAccessor.HttpContext.Items[$"Activity_{request.ActivityId}"];

            //dont have access to IUserAccessor(HttpContext) as using SignalR(webSockets)
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.UserName == request.Username);

            var comment = new Comment {
                Author = user,
                Activity = activity,
                Body = request.Body,
                CreatedAt = DateTime.Now
            };

            activity.Comments.Add(comment);

            var success = await _dbContext.SaveChangesAsync() > 0;

            if (success) return _mapper.Map<CommentDto>(comment);

            throw new Exception("Problem adding comment");
        }
    }
}