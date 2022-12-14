using Application.Common.Interfaces;
using MediatR;

namespace Application.Auth;

public static class CurrentUser {
  public class Query : IRequest<UserDto> { }

  internal class Handler : IRequestHandler<Query, UserDto> {
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IIdentityService currUserService;

    public Handler(IJwtGenerator jwtGenerator, IIdentityService currUserService) {
      this.currUserService = currUserService;
      _jwtGenerator = jwtGenerator;
    }

    public async Task<UserDto> Handle(Query request, CancellationToken ct) {
      var user = await currUserService.GetCurrUserProfileAsync(ct);

      return new UserDto {
        DisplayName = user.DisplayName,
        Username = user.Id,
        Token = _jwtGenerator.CreateToken(user),
        Image = user!.MainPhotoUrl
      };
    }
  }
}