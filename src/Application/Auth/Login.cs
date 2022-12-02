using System.Net;
using Application.Errors;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Auth;

public static class Login {

  public class QueryValidator : AbstractValidator<Query> {
    public QueryValidator() {
      RuleFor(x => x.Email).NotEmpty();
      RuleFor(x => x.Password).NotEmpty();
    }
  }

  public class Handler : IRequestHandler<Query, User> {
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtGenerator _jwtGenerator;

    public Handler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtGenerator jwtGenerator) {
      _jwtGenerator = jwtGenerator;
      _signInManager = signInManager;
      _userManager = userManager;
    }

    public async Task<User> Handle(Query request, CancellationToken ct) {
      var user = await _userManager.FindByEmailAsync(request.Email);

      if (user == null)
        throw new RestException(HttpStatusCode.Unauthorized);

      var result = await _signInManager
          .CheckPasswordSignInAsync(user, request.Password, false);

      if (result.Succeeded) {
        // generate token
        return new User {
          DisplayName = user.DisplayName,
          Token = _jwtGenerator.CreateToken(user),
          Username = user.Id,
          Image = user.MainPhotoUrl
        };
      }

      throw new RestException(HttpStatusCode.Unauthorized);
    }
  }

  public record Query(string Email, string Password): IRequest<User>;
}