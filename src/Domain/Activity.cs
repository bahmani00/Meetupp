using Domain.Common;

namespace Domain;

public class Activity : Entity {
  public Guid Id { get; set; }

  public string Title { get; set; }

  public string Description { get; set; }
  public string Category { get; set; }
  public DateTime Date { get; set; }
  public string City { get; set; }
  public string Venue { get; set; }

  //use virtial to efcore lazy loading rather eagerly loading
  public virtual ICollection<UserActivity> UserActivities { get; set; }
  public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>(0);
}