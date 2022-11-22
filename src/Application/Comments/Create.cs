using Application.Auth;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;
using static Application.Errors.RestException;

namespace Application.Comments;

public static class Create {

  public class Command : IRequest<CommentDto> {
    public string Body { get; set; }
    public Guid ActivityId { get; set; }
    public string Username { get; set; }
  }

  public class CommandValidator : AbstractValidator<Command> {
    private readonly DataContext dbContext;
    private readonly IHttpContextAccessor httpContextAccessor;

    public CommandValidator(DataContext dbContext, IHttpContextAccessor httpContextAccessor) {
      this.dbContext = dbContext;
      this.httpContextAccessor = httpContextAccessor;

      RuleFor(x => x.ActivityId).Cascade(CascadeMode.Stop)
          .NotNull().WithMessage("ActivityId can not be null")
          .NotEmpty().WithMessage("ActivityId can not be empty")
          .Must(ExistsInDatabase).WithMessage("The activity does not exists");
      RuleFor(x => x.Username).NotEmpty();
      RuleFor(x => x.Body).NotEmpty();
    }

    private bool ExistsInDatabase(Guid activityId) {
      // var activity = dbContext.Activities.Find(activityId);

      // if (activity == null)
      //   return false;

      // httpContextAccessor.HttpContext.Items[$"Activity_{activityId}"] = activity;

      return true;
    }
  }

  public class Handler : IRequestHandler<Command, CommentDto> {
    private readonly DataContext dbContext;
    private readonly IMapper mapper;
    private readonly IHttpContextAccessor httpContextAccessor;

    public Handler(DataContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor) {
      this.dbContext = dbContext;
      this.mapper = mapper;
      this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<CommentDto> Handle(Command request, CancellationToken ct) {
      var activity = await dbContext.Activities
        .Include(x => x.Comments)
        .SingleOrDefaultAsync(x => x.Id == request.ActivityId, ct);
      ThrowIfNotFound(activity, new { Activity = "Not found"});

      //dont have access to IUserAccessor(HttpContext) as using SignalR(webSockets)
      var user = await dbContext.Users
        .Include(x => x.Photos)
        .SingleOrDefaultAsync(x => x.UserName == request.Username, ct);

      var comment = new Comment {
        AuthorId = user.Id,
        Author = user,
        ActivityId = request.ActivityId,
        Body = request.Body,
        CreatedAt = DateTime.Now
      };

      activity.Comments.Add(comment);

      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return mapper.Map<CommentDto>(comment);

      throw new Exception("Problem adding comment");
    }
  }
}