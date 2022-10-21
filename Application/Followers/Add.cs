using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Auth;
using Application.Errors;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers;

public class Add {
  public class Command : IRequest {
    public string Username { get; set; }
  }

  public class Handler : IRequestHandler<Command> {
    private readonly DataContext _dbContext;
    private readonly IUserAccessor _userAccessor;
    public Handler(DataContext dbContext, IUserAccessor userAccessor) {
      _userAccessor = userAccessor;
      _dbContext = dbContext;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken) {
      var observer = await _dbContext.Users.SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentUsername());

      var target = await _dbContext.Users.SingleOrDefaultAsync(x => x.UserName == request.Username);

      if (target == null)
        throw new RestException(HttpStatusCode.NotFound, new { User = "Not found" });

      var following = await _dbContext.Followings.SingleOrDefaultAsync(x => x.ObserverId == observer.Id && x.TargetId == target.Id);

      if (following != null)
        throw new RestException(HttpStatusCode.BadRequest, new { User = "You are already following this user" });

      if (following == null) {
        following = new UserFollowing {
          Observer = observer,
          Target = target
        };

        _dbContext.Followings.Add(following);
      }

      var success = await _dbContext.SaveChangesAsync() > 0;

      if (success) return Unit.Value;

      throw new Exception("Problem saving followX");
    }
  }
}