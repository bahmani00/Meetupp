using Microsoft.AspNetCore.Identity;

namespace Domain;

public class AppUser : IdentityUser {
  public string DisplayName { get; set; } = null!;

  public string? Bio { get; set; }

  //use virtial to do efcore lazy loading rather eagerly
  public virtual ICollection<UserActivity> UserActivities { get; set; } = new HashSet<UserActivity>(0);
  public virtual ICollection<Photo> Photos { get; set; } = new HashSet<Photo>(0);

  public virtual ICollection<UserFollowing> Followings { get; set; } = new HashSet<UserFollowing>(0);
  public virtual ICollection<UserFollowing> Followers { get; set; } = new HashSet<UserFollowing>(0);

  public bool IsFollowing(string userId) =>
    this.Followings?.Any(x => x.TargetId == userId) == true;

  public string? MainPhotoUrl =>
    this.Photos?.FirstOrDefault(x => x.IsMain)?.Url;
}