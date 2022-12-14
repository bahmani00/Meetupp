using Microsoft.AspNetCore.Http;

namespace Application.Common;

public static class FormFileExts {
  public const int ImageMinimumBytes = 512;

  public static bool IsImage(this IFormFile formFile) {
    //  Check the image mime types
    if (!string.Equals(formFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
        !string.Equals(formFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
        !string.Equals(formFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
        !string.Equals(formFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
        !string.Equals(formFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
        !string.Equals(formFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase)) {
      return false;
    }

    //  Check the image extension
    var fileExtension = Path.GetExtension(formFile.FileName);
    if (!string.Equals(fileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
        && !string.Equals(fileExtension, ".png", StringComparison.OrdinalIgnoreCase)
        && !string.Equals(fileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
        && !string.Equals(fileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase)) {
      return false;
    }

    //Check whether the image size exceeding the limit or not
    if (formFile.Length < ImageMinimumBytes) return false;

    //try {
    //  //  Try to instantiate new Bitmap, if .NET will throw exception
    //  //  we can assume that it's not a valid image
    //  //This call site is reachable on all platforms. 'Image.FromStream(Stream)' is only supported on: 'windows'
    //  var img = Image.FromStream(formFile.OpenReadStream());      

    //} catch {
    //  return false;
    //}

    return true;
  }
}