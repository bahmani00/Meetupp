using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Identity;

namespace Persistence
{
  public class DbSeeder
  {
    public static async Task SeedAsync(DataContext dbContext, UserManager<AppUser> userManager)
    {
        if (!userManager.Users.Any())
        {
            var users = new List<AppUser>
            {
                new AppUser
                {
                    Id = "a",
                    DisplayName = "Bob",
                    UserName = "bob",
                    Email = "bob@test.com",
                    Photos = new [] {new Photo {Id="1", IsMain = true , Url = "https://res.cloudinary.com/stankansas/image/upload/v1609119885/pcykcdlnyjbckrobnzwb.jpg"}}
                },
                new AppUser
                {
                    Id = "b",
                    DisplayName = "Jane",
                    UserName = "jane",
                    Email = "jane@test.com",
                    Photos = new [] {new Photo {Id="2", IsMain = true , Url = "https://res.cloudinary.com/stankansas/image/upload/v1609119965/yglovzkycojx7f0zafgh.jpg"}}
                },
                new AppUser
                {
                    Id = "c",
                    DisplayName = "Tom",
                    UserName = "tom",
                    Email = "tom@test.com",
                    Photos = new [] {new Photo {Id="3", IsMain = true , Url = "https://res.cloudinary.com/stankansas/image/upload/v1609120019/wdxbk5qjkettlxnvzmy5.jpg"}}
                },
            };

            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }

        if (!dbContext.Activities.Any())
        {
            var activities = new List<Activity>
            {
                new Activity
                {
                    Title = "Past Activity 1",
                    Date = DateTime.Now.AddMonths(-2),
                    Description = "Activity 2 months ago",
                    Category = "drinks",
                    City = "London",
                    Venue = "Pub",
                    UserActivities = new List<UserActivity>
                    {
                        new UserActivity
                        {
                            AppUserId = "a",
                            IsHost = true,
                            DateJoined = DateTime.Now.AddMonths(-2)
                        }
                    }
                },
                new Activity
                {
                    Title = "Past Activity 2",
                    Date = DateTime.Now.AddMonths(-1),
                    Description = "Activity 1 month ago",
                    Category = "coding",
                    City = "Paris",
                    Venue = "The Louvre",
                    UserActivities = new List<UserActivity>
                    {
                        new UserActivity
                        {
                            AppUserId = "b",
                            IsHost = true,
                            DateJoined = DateTime.Now.AddMonths(-1)
                        },
                        new UserActivity
                        {
                            AppUserId = "a",
                            IsHost = false,
                            DateJoined = DateTime.Now.AddMonths(-1)
                        },
                    }
                },
                new Activity
                {
                    Title = "Lets Dance",
                    Date = DateTime.Now.AddMonths(-3),
                    Description = "Lets dance like nobody is watching us!",
                    Category = "music",
                    City = "Montreal",
                    Venue = "Cresent",
                    UserActivities = new List<UserActivity>
                    {
                        new UserActivity
                        {
                            AppUserId = "b",
                            IsHost = true,
                            DateJoined = DateTime.Now.AddMonths(-3)
                        },
                        new UserActivity
                        {
                            AppUserId = "a",
                            IsHost = false,
                            DateJoined = DateTime.Now.AddMonths(-3).AddDays(1)
                        },
                    },
                    Comments = new List<Comment>
                    {
                        new Comment
                        {
                            Id = Guid.NewGuid(),
                            Body = "How artistic!",
                            AuthorId = "a",
                            CreatedAt = DateTime.Now.AddMonths(-3).AddDays(2)
                        },
                        new Comment
                        {
                            Id = Guid.NewGuid(),
                            Body = "Awesome, lets do it",
                            AuthorId = "c",
                            CreatedAt = DateTime.Now.AddMonths(-3).AddDays(4)
                        },
                        new Comment
                        {
                            Id = Guid.NewGuid(),
                            Body = "Very tempting, going",
                            AuthorId = "b",
                            CreatedAt = DateTime.Now.AddMonths(-3).AddDays(3)
                        }
                    }
                },
                new Activity
                {
                    Title = "Future Activity 2",
                    Date = DateTime.Now.AddMonths(2),
                    Description = "Activity 2 months in future",
                    Category = "food",
                    City = "London",
                    Venue = "Jamies Italian",
                    UserActivities = new List<UserActivity>
                    {
                        new UserActivity
                        {
                            AppUserId = "c",
                            IsHost = true,
                            DateJoined = DateTime.Now.AddMonths(2)
                        },
                        new UserActivity
                        {
                            AppUserId = "a",
                            IsHost = false,
                            DateJoined = DateTime.Now.AddMonths(2)
                        },
                    }
                },
                new Activity
                {
                    Title = "Future Activity 3",
                    Date = DateTime.Now.AddMonths(3),
                    Description = "Activity 3 months in future",
                    Category = "drinks",
                    City = "NY",
                    Venue = "Pub",
                    UserActivities = new List<UserActivity>
                    {
                        new UserActivity
                        {
                            AppUserId = "b",
                            IsHost = true,
                            DateJoined = DateTime.Now.AddMonths(3)
                        },
                        new UserActivity
                        {
                            AppUserId = "c",
                            IsHost = false,
                            DateJoined = DateTime.Now.AddMonths(3)
                        },
                    }
                },
                new Activity
                {
                    Title = "Future Activity 4",
                    Date = DateTime.Now.AddMonths(4),
                    Description = "Activity 4 months in future",
                    Category = "culture",
                    City = "Toronto",
                    Venue = "Canada Museum",
                    UserActivities = new List<UserActivity>
                    {
                        new UserActivity
                        {
                            AppUserId = "a",
                            IsHost = true,
                            DateJoined = DateTime.Now.AddMonths(4)
                        }
                    }
                },
                new Activity
                {
                    Title = "Future Activity 5",
                    Date = DateTime.Now.AddMonths(5),
                    Description = "Activity 5 months in future",
                    Category = "drinks",
                    City = "London",
                    Venue = "Punch and Judy",
                    UserActivities = new List<UserActivity>
                    {
                        new UserActivity
                        {
                            AppUserId = "c",
                            IsHost = true,
                            DateJoined = DateTime.Now.AddMonths(5)
                        },
                        new UserActivity
                        {
                            AppUserId = "b",
                            IsHost = false,
                            DateJoined = DateTime.Now.AddMonths(5)
                        },
                    }
                },
                new Activity
                {
                    Title = "Future Activity 6",
                    Date = DateTime.Now.AddMonths(6),
                    Description = "Activity 6 months in future",
                    Category = "music",
                    City = "London",
                    Venue = "O2 Arena",
                    UserActivities = new List<UserActivity>
                    {
                        new UserActivity
                        {
                            AppUserId = "a",
                            IsHost = true,
                            DateJoined = DateTime.Now.AddMonths(6)
                        },
                        new UserActivity
                        {
                            AppUserId = "b",
                            IsHost = false,
                            DateJoined = DateTime.Now.AddMonths(6)
                        },
                    }
                },
                new Activity
                {
                    Title = "Future Activity 7",
                    Date = DateTime.Now.AddMonths(7),
                    Description = "Activity 7 months in future",
                    Category = "travel",
                    City = "Berlin",
                    Venue = "All",
                    UserActivities = new List<UserActivity>
                    {
                        new UserActivity
                        {
                            AppUserId = "a",
                            IsHost = true,
                            DateJoined = DateTime.Now.AddMonths(7)
                        },
                        new UserActivity
                        {
                            AppUserId = "c",
                            IsHost = false,
                            DateJoined = DateTime.Now.AddMonths(7)
                        },
                    }
                },
                new Activity
                {
                    Title = "Future Activity 8",
                    Date = DateTime.Now.AddMonths(8),
                    Description = "Activity 8 months in future",
                    Category = "fun",
                    City = "Montreal",
                    Venue = "Pub",
                    UserActivities = new List<UserActivity>
                    {
                        new UserActivity
                        {
                            AppUserId = "b",
                            IsHost = true,
                            DateJoined = DateTime.Now.AddMonths(8)
                        },
                        new UserActivity
                        {
                            AppUserId = "a",
                            IsHost = false,
                            DateJoined = DateTime.Now.AddMonths(8)
                        },
                    }
                }
            };

            await dbContext.Activities.AddRangeAsync(activities);
            await dbContext.SaveChangesAsync();
        }
    }
  }
}