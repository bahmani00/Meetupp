namespace Domain;

public class Photo {
  public Photo() {
  }

  public Photo(string id, bool isMain, string url) {
    PublicId = Id = id;
    IsMain = isMain;
    Url = url;
  }

  public string Id { get; set; } = null!;

  public string PublicId { get; set; } = null!;

  public string Url { get; set; } = null!;

  //is main photo for the user?
  public bool IsMain { get; set; }
}