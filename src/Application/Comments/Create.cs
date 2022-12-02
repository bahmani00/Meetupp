using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments;

public static class Create {

  internal class Handler : IRequestHandler<Command, CommentDto> {
    private readonly DataContext dbContext;
    private readonly IMapper mapper;
    private readonly HttpContext httpContext;

    public Handler(DataContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor) {
      this.dbContext = dbContext;
      this.mapper = mapper;
      this.httpContext = httpContextAccessor.HttpContext;
    }

    public async Task<CommentDto> Handle(Command request, CancellationToken ct) {
      var user = (AppUser)httpContext.Items[$"user_{request.UserId}"];

      var comment = new Comment {
        AuthorId = user.Id,
        ActivityId = request.ActivityId,
        Body = request.Body,
        CreatedAt = DateTime.Now
      };

      dbContext.Comments.Add(comment);

      var success = await dbContext.SaveChangesAsync(ct) > 0;
      comment.Author = user;
      if (success) return mapper.Map<CommentDto>(comment);

      throw new Exception("Problem adding comment");
    }
  }

  public class CommandValidator : AbstractValidator<Command> {
    private readonly DataContext dbContext;
    private readonly HttpContext httpContext;

    public CommandValidator(DataContext dbContext, IHttpContextAccessor httpContextAccessor) {
      this.dbContext = dbContext;
      this.httpContext = httpContextAccessor.HttpContext;

      RuleFor(x => x.ActivityId).Cascade(CascadeMode.Stop)
          .NotEmpty()
          .Must(ActivityExistsInDb).WithMessage("Activity not found");
      RuleFor(x => x.UserId).Cascade(CascadeMode.Stop)
        .NotEmpty()
        .Must(UserExistsInDb).WithMessage("User not found");
      RuleFor(x => x.Body).NotEmpty();
    }

    private bool ActivityExistsInDb(Guid activityId) =>
      dbContext.Activities.Find(activityId) != null;

    private bool UserExistsInDb(string userId) {
      //dont have access to ICurrUserService(HttpContext) as using SignalR(webSockets)
      var user = dbContext.Users
        .AsNoTracking()
        .Include(x => x.Photos)
        .SingleOrDefault(x => x.Id == userId);

      httpContext.Items[$"user_{userId}"] = user;

      return user != null;
    }
  }

  public sealed record Command(
    Guid ActivityId,
    string UserId,
    string Body) : IRequest<CommentDto> {
  }
}