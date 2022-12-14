namespace Domain.Common;

public abstract class Entity {
  public string CreatedById { get; set; } = null!;
  public AppUser CreatedBy { get; set; } = null!;

  public DateTime CreatedOn { get; set; }

  public string? ModifiedById { get; set; }
  public AppUser? ModifiedBy { get; set; }

  public DateTime? ModifiedOn { get; set; }
}