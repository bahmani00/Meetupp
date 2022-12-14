using System.Text.Json.Serialization;
using Application.Photos;
using Domain;

namespace Application.Profiles;

public class Profile {
  public string DisplayName { get; set; } = null!;
  public string Username { get; set; } = null!;
  public string? Image { get; set; }
  public string? Bio { get; set; }

  [JsonPropertyNameAttribute("following")]
  public bool IsFollowed { get; set; }
  public int FollowersCount { get; set; }
  public int FollowingCount { get; set; }
  public ICollection<PhotoDto>? Photos { get; set; }

  public static Profile From(AppUser user, AppUser currUser) =>
    new() {
      DisplayName = user!.DisplayName,
      Username = user!.UserName!,
      Image = user.MainPhotoUrl,
      Photos = user.Photos?.Select(PhotoDto.From)?.ToList(),
      Bio = user.Bio,
      FollowersCount = user.Followers.Count,
      FollowingCount = user.Followings.Count,
      IsFollowed = currUser.IsFollowing(user.Id)
    };
}