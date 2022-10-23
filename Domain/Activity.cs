using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Activity {
    public Guid Id { get; set; }
    
    [StringLength(50)]
    public string Title { get; set; }
    
    [StringLength(500)]
    public string Description { get; set; }
    [StringLength(50)]
    public string Category { get; set; }
    public DateTime Date { get; set; }
    [StringLength(50)]
    public string City { get; set; }
    [StringLength(50)]
    public string Venue { get; set; }

    //use virtial to efcore lazy loading rather eagerly loading
    public virtual ICollection<UserActivity> UserActivities { get; set; }
    public virtual ICollection<Comment> Comments { get; set; }
}