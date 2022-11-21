using System.Text.Json.Serialization;
using Domain;

namespace Application.Profiles;

public class Profile {
  public string DisplayName { get; set; }
  public string Username { get; set; }
  public string Image { get; set; }
  public string Bio { get; set; }

  [JsonPropertyNameAttribute("following")]
  public bool IsFollowed { get; set; }
  public int FollowersCount { get; set; }
  public int FollowingCount { get; set; }
  public ICollection<Photo> Photos { get; set; }

  public static Profile From(AppUser user, AppUser loggedInUser) =>
    new Profile {
      DisplayName = user.DisplayName,
      Username = user.UserName,
      Image = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
      Photos = user.Photos,
      Bio = user.Bio,
      FollowersCount = user.Followers.Count,
      FollowingCount = user.Followings.Count,
      IsFollowed = loggedInUser.Followings.Any(x => x.TargetId == user.Id)
    };
}