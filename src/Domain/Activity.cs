using Domain.Common;

namespace Domain;

public class Activity : Entity {
  public Guid Id { get; set; }

  public string Title { get; set; } = null!;
  public string? Description { get; set; }

  public string Category { get; set; } = null!;

  public DateTime Date { get; set; }

  public string City { get; set; } = null!;
  public string Venue { get; set; } = null!;

  //use virtial to efcore lazy loading rather eagerly loading
  public virtual ICollection<UserActivity> UserActivities { get; set; } = new HashSet<UserActivity>(0);
  public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>(0);
}