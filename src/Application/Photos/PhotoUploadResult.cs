using Domain;

namespace Application.Photos;

public class PhotoUploadResult {
  public string PublicId { get; set; }
  public string Url { get; set; }

  internal Photo ToEntity(AppUser user) =>
    new() {
      Url = this.Url,
      Id = this.PublicId,
      IsMain = !user.Photos.Any(x => x.IsMain);
    };
}