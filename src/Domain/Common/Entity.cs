using System.ComponentModel.DataAnnotations;

namespace Domain.Common;

public abstract class Entity {
  [Required]
  public string CreatedById { get; set; }
  public AppUser CreatedBy { get; set; }

  [Required]
  public DateTimeOffset CreatedOn { get; set; }

  public string ModifiedById { get; set; }
  public AppUser ModifiedBy { get; set; }

  public DateTimeOffset ModifiedOn { get; set; }
}