using System.ComponentModel.DataAnnotations;
using Domain.Common;

namespace Domain;

public class Comment : Entity {
  public Guid Id { get; set; }

  [Required]
  public string Body { get; set; }

  public Guid ActivityId { get; set; }
  public virtual Activity Activity { get; set; }
}