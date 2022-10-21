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
        if (!dbContext.Users.Any())
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
                new AppUser
                {
                    Id = "d2",
                    DisplayName = "Tom 2",
                    UserName = "tom2",
                    Email = "tom2@test.com",
                    Photos = new [] {new Photo {Id="4", IsMain = true , Url = "https://res.cloudinary.com/stankansas/image/upload/v1609120019/wdxbk5qjkettlxnvzmy5.jpg"}}
                },
                new AppUser
                {
                    Id = "c3",
                    DisplayName = "Tom 3",
                    UserName = "tom3",
                    Email = "tom3@test.com",
                    Photos = new [] {new Photo {Id="5", IsMain = true , Url = "https://res.cloudinary.com/stankansas/image/upload/v1609120019/wdxbk5qjkettlxnvzmy5.jpg"}}
                },
                new AppUser
                {
                    Id = "c4",
                    DisplayName = "Tom 4",
                    UserName = "tom4",
                    Email = "tom4@test.com",
                    Photos = new [] {new Photo {Id="6", IsMain = true , Url = "https://res.cloudinary.com/stankansas/image/upload/v1609120019/wdxbk5qjkettlxnvzmy5.jpg"}}
                },
                new AppUser
                {
                    Id = "c5",
                    DisplayName = "Tom 5",
                    UserName = "tom5",
                    Email = "tom5@test.com",
                    Photos = new [] {new Photo {Id="7", IsMain = true , Url = "https://res.cloudinary.com/stankansas/image/upload/v1609120019/wdxbk5qjkettlxnvzmy5.jpg"}}
                },
                                new AppUser
                {
                    Id = "c6",
                    DisplayName = "Tom 6",
                    UserName = "tom6",
                    Email = "tom6@test.com",
                    Photos = new [] {new Photo {Id="8", IsMain = true , Url = "https://res.cloudinary.com/stankansas/image/upload/v1609120019/wdxbk5qjkettlxnvzmy5.jpg"}}
                },
                                new AppUser
                {
                    Id = "c7",
                    DisplayName = "Tom 007",
                    UserName = "tom7",
                    Email = "tom7@test.com",
                    Photos = new [] {new Photo {Id="9", IsMain = true , Url = "https://res.cloudinary.com/stankansas/image/upload/v1609120019/wdxbk5qjkettlxnvzmy5.jpg"}}
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

            for(var i = 10; i <= 100; ++i){
                activities.Add(new Activity {
                    Title = "Future Activity " + i,
                    Date = DateTime.Now.AddMonths(1),
                    Description = $"Activity {i} months in future",
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
                });
            }

            await dbContext.Activities.AddRangeAsync(activities);
            await dbContext.SaveChangesAsync();
        }

        if (!dbContext.Followings.Any())
        {
            var followings = new List<UserFollowing>
            {
                new UserFollowing
                {
                    Observer = userManager.Users.ToList().ElementAt(0),
                    Target = userManager.Users.ToList().ElementAt(1),
                },
                new UserFollowing
                {
                    Observer = userManager.Users.ToList().ElementAt(0),
                    Target = userManager.Users.ToList().ElementAt(2),
                },
                new UserFollowing
                {
                    Observer = userManager.Users.ToList().ElementAt(0),
                    Target = userManager.Users.ToList().ElementAt(3),
                },
                new UserFollowing
                {
                    Observer = userManager.Users.ToList().ElementAt(0),
                    Target = userManager.Users.ToList().ElementAt(4),
                },
                new UserFollowing
                {
                    Observer = userManager.Users.ToList().ElementAt(0),
                    Target = userManager.Users.ToList().ElementAt(5),
                },
                new UserFollowing
                {
                    Observer = userManager.Users.ToList().ElementAt(1),
                    Target = userManager.Users.ToList().ElementAt(2),
                }
            };

            await dbContext.Followings.AddRangeAsync(followings);
            await dbContext.SaveChangesAsync();
        }
    }
  }
}