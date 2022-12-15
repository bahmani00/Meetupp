namespace Domain;

public class UserFollowing {
  public string ObserverId { get; set; } = null!;
  public virtual AppUser Observer { get; set; } = null!;

  public string TargetId { get; set; } = null!;
  public virtual AppUser Target { get; set; } = null!;

  public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}