using System.ComponentModel.DataAnnotations;

namespace Domain;

public class UserActivity {

  [StringLength(50)]
  public string AppUserId { get; set; }

  //use virtial to efcore lazy loading rather eagerly
  public virtual AppUser AppUser { get; set; }
  public Guid ActivityId { get; set; }

  //use virtial to efcore lazy loading rather eagerly
  public virtual Activity Activity { get; set; }
  public DateTime DateJoined { get; set; }
  public bool IsHost { get; set; }

  public static UserActivity CreateHostActivity(AppUser user, Activity activity) =>
    new() {
      AppUser = user,
      Activity = activity,
      IsHost = true,
      DateJoined = DateTime.Now
    };
}