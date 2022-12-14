namespace Domain;

public class UserActivity {
  public string AppUserId { get; set; } = null!;
  public virtual AppUser AppUser { get; set; } = null!;

  public Guid ActivityId { get; set; }
  //use virtial to efcore lazy loading rather eagerly
  public virtual Activity Activity { get; set; } = null!;

  public DateTime DateJoined { get; set; }

  public bool IsHost { get; set; }

  public static UserActivity Create(AppUser user, Activity activity, bool isHost = false) =>
    Create(user.Id, activity.Id, isHost);

  public static UserActivity Create(string userId, Guid activityId, bool isHost = false) =>
    new() {
      AppUserId = userId,
      ActivityId = activityId,
      IsHost = isHost,
      DateJoined = DateTime.Now
    };
}