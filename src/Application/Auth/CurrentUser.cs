using MediatR;

namespace Application.Auth;

public static class CurrentUser {
  public class Query : IRequest<User> { }

  internal class Handler : IRequestHandler<Query, User> {
    private readonly IJwtGenerator _jwtGenerator;
    private readonly ICurrUserService currUserService;

    public Handler(IJwtGenerator jwtGenerator, ICurrUserService currUserService) {
      this.currUserService = currUserService;
      _jwtGenerator = jwtGenerator;
    }

    public async Task<User> Handle(Query request, CancellationToken ct) {
      var user = await currUserService.GetCurrUserAsync(ct);

      return new User {
        DisplayName = user.DisplayName,
        Username = user.UserName,
        Token = _jwtGenerator.CreateToken(user),
        Image = user.MainPhotoUrl
      };
    }
  }
}