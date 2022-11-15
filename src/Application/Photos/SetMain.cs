using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Auth;
using Application.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos;

public static class SetMain {
  public class Command : IRequest {
    public string Id { get; set; }
  }

  public class Handler : IRequestHandler<Command> {
    private readonly DataContext dbContext;
    private readonly IUserAccessor userAccessor;

    public Handler(DataContext dbContext, IUserAccessor userAccessor) {
      this.dbContext = dbContext;
      this.userAccessor = userAccessor;
    }

    public async Task<Unit> Handle(Command request, CancellationToken ct) {
      var user = await dbContext.Users
        .SingleOrDefaultAsync(x => x.UserName == userAccessor.GetCurrentUsername(), ct);

      var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);

      if (photo == null)
        RestException.ThrowNotFound(new { Photo = "Not found" });

      var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

      currentMain.IsMain = false;
      photo.IsMain = true;

      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return Unit.Value;

      throw new Exception("Problem setting main photo");
    }
  }
}