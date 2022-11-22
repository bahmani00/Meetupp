using Application.Photos;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IPhotoAccessor {
  string Provider { get; }
  PhotoUploadResult AddPhoto(IFormFile file);
  string DeletePhoto(string publicId);
}