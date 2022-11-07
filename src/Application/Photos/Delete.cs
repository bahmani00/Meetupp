using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Auth;
using Application.Errors;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos;

public class Delete {
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
      var user = await dbContext.Users.SingleOrDefaultAsync(x => x.UserName == userAccessor.GetCurrentUsername(), ct);

      var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);

      if (photo == null)
        throw new RestException(HttpStatusCode.NotFound, new { Photo = "Not found" });

      if (photo.IsMain)
        throw new RestException(HttpStatusCode.BadRequest, new { Photo = "You cannot delete your main photo" });

      var result = photoAccessor.DeletePhoto(photo.Id);

      if (result == null)
        throw new Exception("Problem deleting photo");

      user.Photos.Remove(photo);

#pragma warning disable CA2016 // Forward the 'CancellationToken' parameter to methods
      var success = await dbContext.SaveChangesAsync() > 0;
#pragma warning restore CA2016 // Forward the 'CancellationToken' parameter to methods

      if (success) return Unit.Value;

      throw new Exception("Problem deleting photo");
    }
  }
}