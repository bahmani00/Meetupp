using Application.Common.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth;

public static class Login {

  public class QueryValidator : AbstractValidator<Query> {
    private readonly IAppDbContext dbContext;
    private readonly SignInManager<AppUser> signInManager;
    private readonly IHttpContextAccessor httpContextAccessor;

    public QueryValidator(
      IAppDbContext dbContext,
      SignInManager<AppUser> signInManager,
      IHttpContextAccessor httpContextAccessor) {
      this.dbContext = dbContext;
      this.signInManager = signInManager;
      this.httpContextAccessor = httpContextAccessor;

      RuleFor(x => x.Email)
        .NotEmpty();
      RuleFor(x => x.Password)
        .NotEmpty();

      RuleFor(x => x)
        .Must(IsPasswordValid).OverridePropertyName("Credintials").WithMessage("Invalid credintials");
    }

    private bool IsPasswordValid(Query request) {
      //dont have access to ICurrUserService(HttpContext) as using SignalR(webSockets)
      var user = dbContext.Users
        .AsNoTracking()
        .Include(x => x.Photos)
        .SingleOrDefault(x => x.Email == request.Email);

      httpContextAccessor!.HttpContext!.Items[$"user_{request.Email}"] = user;

      if (user == null) return false;

      //dont have access to ICurrUserService(HttpContext) as using SignalR(webSockets)
      var result = signInManager
          .CheckPasswordSignInAsync(user, request.Password, false).Result;

      return result.Succeeded;
    }
  }

  public class Handler : IRequestHandler<Query, UserDto> {
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IJwtGenerator _jwtGenerator;

    public Handler(
      IHttpContextAccessor httpContextAccessor,
      IJwtGenerator jwtGenerator) {
      this.httpContextAccessor = httpContextAccessor;
      _jwtGenerator = jwtGenerator;
    }

    public async Task<UserDto> Handle(Query request, CancellationToken ct) {
      var user = httpContextAccessor!.HttpContext!.Items[$"user_{request.Email}"] as AppUser;

      await Task.CompletedTask;

      // generate token
      return new UserDto {
        DisplayName = user!.DisplayName,
        Token = _jwtGenerator.CreateToken(user),
        Username = user.Id,
        Image = user.MainPhotoUrl
      };
    }
  }

  public record Query(string Email, string Password) : IRequest<UserDto>;
}