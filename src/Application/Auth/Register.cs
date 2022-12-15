using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Application.Common.Exceptions.RestException;

namespace Application.Auth;

public static class Register {
  public class Command : IRequest<UserDto> {
    public string DisplayName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
  }

  public class CommandValidator : AbstractValidator<Command> {
    public CommandValidator() {
      RuleFor(x => x.DisplayName).NotEmpty();
      RuleFor(x => x.Username).NotEmpty();
      RuleFor(x => x.Email).NotEmpty().EmailAddress();
      RuleFor(x => x.Password).Password();
    }
  }

  internal class Handler : IRequestHandler<Command, UserDto> {
    private readonly UserManager<AppUser> userManager;
    private readonly IJwtGenerator jwtGenerator;

    public Handler(UserManager<AppUser> userManager, IJwtGenerator jwtGenerator) {
      this.jwtGenerator = jwtGenerator;
      this.userManager = userManager;
    }

    public async Task<UserDto> Handle(Command request, CancellationToken ct) {
      var exists = await userManager.Users
        .AnyAsync(x => x.Email == request.Email || x.UserName == request.Username, ct);
      ThrowIfBadRequest(exists, new { Email = "Email or Username already exists" });

      var user = new AppUser {
        DisplayName = request.DisplayName,
        Email = request.Email,
        UserName = request.Username
      };

      var result = await userManager.CreateAsync(user, request.Password);

      if (result.Succeeded) {
        return new UserDto {
          DisplayName = user.DisplayName,
          Token = jwtGenerator.CreateToken(user),
          Username = user.UserName,
          Image = user.MainPhotoUrl
        };
      }

      throw new Exception("Problem creating user");
    }
  }
}