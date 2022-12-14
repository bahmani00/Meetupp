using Application.Common.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Comments;

public static class Create {

  internal class Handler : IRequestHandler<Command, CommentDto> {
    private readonly IAppDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IHttpContextAccessor httpContextAccessor;

    public Handler(IAppDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor) {
      this.dbContext = dbContext;
      this.mapper = mapper;
      this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<CommentDto> Handle(Command request, CancellationToken ct) {
      var user = httpContextAccessor!.HttpContext!.Items[$"user_{request.UserId}"] as AppUser;

      var comment = new Comment {
        CreatedById = user!.Id,
        ActivityId = request.ActivityId,
        Body = request.Body,
        CreatedOn = DateTime.Now
      };

      dbContext.Comments.Add(comment);

      var success = await dbContext.SaveChangesAsync(ct) > 0;
      comment.CreatedBy = user;
      if (success) return mapper.Map<CommentDto>(comment);

      throw new Exception("Problem adding comment");
    }
  }

  public class CommandValidator : AbstractValidator<Command> {
    private readonly IAppDbContext dbContext;
    private readonly IHttpContextAccessor httpContextAccessor;

    public CommandValidator(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor) {
      this.dbContext = dbContext;
      this.httpContextAccessor = httpContextAccessor;

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
        .TagWithCallSite()
        .SingleOrDefault(x => x.Id == userId);

      httpContextAccessor!.HttpContext!.Items[$"user_{userId}"] = user;

      return user != null;
    }
  }

  public sealed record Command(Guid ActivityId, string UserId, string Body) : IRequest<CommentDto>;
}