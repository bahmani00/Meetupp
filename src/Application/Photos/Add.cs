using Application.Auth;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos;

public static class Add {
  public class Command : IRequest<Photo> {
    public IFormFile File { get; set; }
  }

  public class Handler : IRequestHandler<Command, Photo> {
    private readonly DataContext dbContext;
    private readonly IUserAccessor userAccessor;
    private readonly IPhotoAccessor photoAccessor;

    public Handler(DataContext dbContext, IUserAccessor userAccessor, IPhotoAccessor photoAccessor) {
      this.dbContext = dbContext;
      this.photoAccessor = photoAccessor;
      this.userAccessor = userAccessor;
    }

    public async Task<Photo> Handle(Command request, CancellationToken ct) {
      var photoUploadResult = photoAccessor.AddPhoto(request.File);

      var user = await dbContext.Users
        .Include(x => x.Photos)
        .SingleOrDefaultAsync(x => x.UserName == userAccessor.GetCurrUsername(), ct);

      var photo = photoUploadResult.ToEntity(user);
      user.Photos.Add(photo);

      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return photo;

      throw new Exception("Problem saving photo");
    }
  }
}