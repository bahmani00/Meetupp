using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Auth;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles;

public static class Edit {
  public class Command : IRequest {
    public string DisplayName { get; set; }
    public string Bio { get; set; }
  }

  public class CommandValidator : AbstractValidator<Command> {
    public CommandValidator() {
      RuleFor(x => x.DisplayName).NotEmpty();
    }
  }

  public class Handler : IRequestHandler<Command> {
    private readonly DataContext dbContext;
    private readonly IUserAccessor userAccessor;

    public Handler(DataContext dbContext, IUserAccessor userAccessor) {
      this.dbContext = dbContext;
      this.userAccessor = userAccessor;
    }

    public async Task<Unit> Handle(Command request, CancellationToken ct) {
      var user = await dbContext.Users.SingleOrDefaultAsync(x => x.UserName == userAccessor.GetCurrentUsername(), ct);

      user.DisplayName = request.DisplayName ?? user.DisplayName;
      user.Bio = request.Bio ?? user.Bio;

      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return Unit.Value;

      throw new Exception("Problem editing profile");
    }
  }
}