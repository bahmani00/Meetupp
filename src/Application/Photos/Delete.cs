using Application.Auth;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using static Application.Errors.RestException;

namespace Application.Photos;

public static class Delete {
  public class Command : IRequest {
    public string Id { get; set; }
  }

  public class Handler : IRequestHandler<Command> {
    private readonly DataContext dbContext;
    private readonly IUserAccessor userAccessor;
    private readonly IPhotoAccessor photoAccessor;

    public Handler(DataContext dbContext, IUserAccessor userAccessor, IPhotoAccessor photoAccessor) {
      this.dbContext = dbContext;
      this.photoAccessor = photoAccessor;
      this.userAccessor = userAccessor;
    }

    public async Task<Unit> Handle(Command request, CancellationToken ct) {
      var user = await dbContext.Users
        .Include(x => x.Photos)
        .SingleOrDefaultAsync(x => x.UserName == userAccessor.GetCurrUsername(), ct);

      var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);
      ThrowIfNotFound(photo, new { Photo = "Not found" });
      ThrowIfBadRequest(photo.IsMain, new { Photo = "You cannot delete your main photo" });

      var result = photoAccessor.DeletePhoto(photo.Id);
      if (result == null)
        throw new Exception($"Problem deleting photo from {photoAccessor.Provider}");

      user.Photos.Remove(photo);

#pragma warning disable CA2016 // Forward the 'CancellationToken' parameter to methods
      var success = await dbContext.SaveChangesAsync() > 0;
#pragma warning restore CA2016 // Forward the 'CancellationToken' parameter to methods

      if (success) return Unit.Value;

      throw new Exception("Problem deleting photo");
    }
  }
}