using Domain;

namespace Application.Photos;

public class PhotoUploadResult {
  public string PublicId { get; set; } = null!;
  public string Url { get; set; } = null!;

  internal Photo ToEntity(AppUser user) =>
    new() {
      Url = this.Url,
      Id = this.PublicId,
      PublicId = this.PublicId,
      IsMain = user.Photos.Any(x => x.IsMain) == true
    };
}