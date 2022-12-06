using System.ComponentModel.DataAnnotations;
using Application.Auth;
using Application.Common.Interfaces;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Photos;

public static class Add {
  public class Command : IRequest<Photo> {
    [Required]
    public IFormFile File { get; set; }
  }

  public class Handler : IRequestHandler<Command, Photo> {
    private readonly IAppDbContext dbContext;
    private readonly ICurrUserService currUserService;
    private readonly IPhotoAccessor photoAccessor;

    public Handler(IAppDbContext dbContext, ICurrUserService currUserService, IPhotoAccessor photoAccessor) {
      this.dbContext = dbContext;
      this.photoAccessor = photoAccessor;
      this.currUserService = currUserService;
    }

    public async Task<Photo> Handle(Command request, CancellationToken ct) {
      var photoUploadResult = photoAccessor.AddPhoto(request.File);

      var user = await dbContext.Users
        .Include(x => x.Photos)
        .SingleOrDefaultAsync(x => x.UserName == currUserService.UserId, ct);

      var photo = photoUploadResult.ToEntity(user);
      user.Photos.Add(photo);

      var success = await dbContext.SaveChangesAsync(ct) > 0;

      if (success) return photo;

      throw new Exception("Problem saving photo");
    }
  }
}