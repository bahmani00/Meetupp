using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain;

public class AppUser : IdentityUser {
  [StringLength(50)]
  public string DisplayName { get; set; }

  public string Bio { get; set; }

  //use virtial to do efcore lazy loading rather eagerly
  public virtual ICollection<UserActivity> UserActivities { get; set; }
  public virtual ICollection<Photo> Photos { get; set; } = new HashSet<Photo>(0);

  public virtual ICollection<UserFollowing> Followings { get; set; }
  public virtual ICollection<UserFollowing> Followers { get; set; }

  public bool IsFollowing(string userId) =>
    this.Followings?.Any(x => x.TargetId == userId) == true;

  public string MainPhotoUrl =>
    this.Photos?.FirstOrDefault(x => x.IsMain)?.Url;
}