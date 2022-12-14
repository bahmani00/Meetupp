using Domain.Common;

namespace Domain;

public class Comment : Entity {
  public Guid Id { get; set; }

  public string Body { get; set; } = null!;

  public Guid ActivityId { get; set; }
  public virtual Activity Activity { get; set; } = null!;
}