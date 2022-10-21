using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain;

//[Table("AppUser")]
public class AppUser : IdentityUser {
    [StringLength(50)]
    public string DisplayName { get; set; }

    [StringLength(500)]
    public string Bio { get; set; }

    //use virtial to do efcore lazy loading rather eagerly
    public virtual ICollection<UserActivity> UserActivities { get; set; }
    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>(0);

    public virtual ICollection<UserFollowing> Followings { get; set; }
    public virtual ICollection<UserFollowing> Followers { get; set; }
}