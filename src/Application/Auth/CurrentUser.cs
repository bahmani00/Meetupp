using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Auth;

public static class CurrentUser {
  public class Query : IRequest<User> { }

  public class Handler : IRequestHandler<Query, User> {
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IUserAccessor userAccessor;

    public Handler(UserManager<AppUser> userManager, IJwtGenerator jwtGenerator, IUserAccessor userAccessor) {
      this.userAccessor = userAccessor;
      _jwtGenerator = jwtGenerator;
      _userManager = userManager;
    }

    public async Task<User> Handle(Query request, CancellationToken ct) {
      var user = await _userManager.FindByNameAsync(userAccessor.GetCurrentUsername());

      return new User {
        DisplayName = user.DisplayName,
        Username = user.UserName,
        Token = _jwtGenerator.CreateToken(user),
        Image = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
      };
    }
  }
}