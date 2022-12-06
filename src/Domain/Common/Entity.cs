using System.ComponentModel.DataAnnotations;

namespace Domain.Common;

public abstract class Entity {
  [Required]
  public DateTimeOffset CreatedOn { get; set; }
  [Required]
  public string CreatedBy { get; set; }

  public DateTimeOffset ModifiedOn { get; set; }
  public string ModifiedBy { get; set; }
}