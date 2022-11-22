using Application.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using static Application.Errors.RestException;

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
        .Include(x => x.Photos)
        .SingleOrDefaultAsync(x => x.UserName == userAccessor.GetCurrUsername(), ct);

      var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);
      ThrowIfNotFound(photo, new { Photo = "Not found" });

      var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

      currentMain.IsMain = false;
      photo.IsMain = true;

      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return Unit.Value;

      throw new Exception("Problem setting main photo");
    }
  }
}