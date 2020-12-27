using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        
		//use virtial to do efcore lazy loading rather eagerly
        public virtual ICollection<UserActivity> UserActivities { get; set; }
    }
}