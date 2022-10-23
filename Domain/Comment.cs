using System;
using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Comment {
    public Guid Id { get; set; }
    public string Body { get; set; }

    [StringLength(50)]
    public string AuthorId { get; set; }
    public virtual AppUser Author { get; set; }

    public virtual Activity Activity { get; set; }
    public DateTime CreatedAt { get; set; }
}