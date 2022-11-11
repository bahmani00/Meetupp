using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Auth;

public static class Register {
  public class Command : IRequest<User> {
    public string DisplayName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
  }

  public class CommandValidator : AbstractValidator<Command> {
    public CommandValidator() {
      RuleFor(x => x.DisplayName).NotEmpty();
      RuleFor(x => x.Username).NotEmpty();
      RuleFor(x => x.Email).NotEmpty().EmailAddress();
      RuleFor(x => x.Password).Password();
    }
  }

  public class Handler : IRequestHandler<Command, User> {
    private readonly DataContext dbContext;
    private readonly UserManager<AppUser> userManager;
    private readonly IJwtGenerator jwtGenerator;

    public Handler(DataContext dbContext, UserManager<AppUser> userManager, IJwtGenerator jwtGenerator) {
      this.dbContext = dbContext;
      this.jwtGenerator = jwtGenerator;
      this.userManager = userManager;
    }

    public async Task<User> Handle(Command request, CancellationToken ct) {
      if (await dbContext.Users.Where(x => x.Email == request.Email).AnyAsync(ct))
        throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already exists" });

      if (await dbContext.Users.Where(x => x.UserName == request.Username).AnyAsync(ct))
        throw new RestException(HttpStatusCode.BadRequest, new { Username = "Username already exists" });

      var user = new AppUser {
        DisplayName = request.DisplayName,
        Email = request.Email,
        UserName = request.Username
      };

      var result = await userManager.CreateAsync(user, request.Password);

      if (result.Succeeded) {
        return new User {
          DisplayName = user.DisplayName,
          Token = jwtGenerator.CreateToken(user),
          Username = user.UserName,
          Image = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };
      }

      throw new Exception("Problem creating user");
    }
  }
}