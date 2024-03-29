using Application.Auth;
using Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Profiles;

public static class Edit {

  internal class Handler : IRequestHandler<Command> {
    private readonly IAppDbContext dbContext;
    private readonly ICurrUserService currUserService;

    public Handler(IAppDbContext dbContext, ICurrUserService currUserService) {
      this.dbContext = dbContext;
      this.currUserService = currUserService;
    }

    public async Task<Unit> Handle(Command request, CancellationToken ct) {
      var user = await dbContext.GetUserAsync(currUserService.UserId, ct, true);

      user!.DisplayName = request.DisplayName ?? user.DisplayName;
      user!.Bio = request.Bio ?? user.Bio;

      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return Unit.Value;

      throw new Exception("Problem editing profile");
    }
  }

  public class CommandValidator : AbstractValidator<Command> {
    public CommandValidator() {
      RuleFor(x => x.DisplayName).NotEmpty();
    }
  }

  public record Command(string DisplayName, string Bio) : IRequest;
}