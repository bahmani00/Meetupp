namespace Domain;

public class UserFollowing {
  public string ObserverId { get; set; }
  public virtual AppUser Observer { get; set; }

  public string TargetId { get; set; }
  public virtual AppUser Target { get; set; }

  public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}