using System;

namespace Domain
{
    public class UserActivity
    {
        public string AppUserId { get; set; }
		
		//use virtial to do efcore lazy loading rather eagerly
        public virtual AppUser AppUser { get; set; }
        public Guid ActivityId { get; set; }
		
		//use virtial to do efcore lazy loading rather eagerly
        public virtual Activity Activity { get; set; }
        public DateTime DateJoined { get; set; }
        public bool IsHost { get; set; }
    }
}