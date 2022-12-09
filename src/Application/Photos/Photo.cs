using Domain;

namespace Application.Photos;

public class PhotoDto {

  public string Id { get; set; }

  public string PublicId { get; set; }

  public string Url { get; set; }
  //is main photo for the user?
  public bool IsMain { get; set; }

  internal static PhotoDto From(Photo entity) =>
    new() {
      Id = entity.Id,
      PublicId = entity.PublicId,
      Url = entity.Url,
      IsMain = entity.IsMain,
    };
}