using System.Globalization;
using Domain;
using Microsoft.AspNetCore.Identity;

namespace Persistence;

public static class DbSeeder {
  public static async Task SeedAsync(DataContext dbContext, UserManager<AppUser> userManager) {
    await AddUsers(dbContext, userManager);

    await AddFollowers(dbContext, userManager);

    await AddActivities(dbContext, userManager);
  }

  static readonly Random rand = new();
  static readonly Dictionary<int, (string cat, string desc)> categories = new() {
    { 1, ("drinks", " Let's drink to humanity & peace") },
    { 2, ("culture", "A social gathering potluck")},
    { 3, ("film", "Let's watch a movie and discuss it") },
    { 4, ("food", "Who's hungry to try some exotic food") },
    { 5, ("music", "Lets listen to mucic and maybe dance") },
    { 6, ("travel", "Travel to gether and share our experience") },
    { 7, ("coding", "Come discuss your side projects") },
    { 8, ("fun", "Go funny! 1, 2, 3 be funny!") }
  };
  static readonly Dictionary<int, (string city, string venue)> cities = new() {
    { 1, ("Montreal", "Musée beaux-arts") },
    { 2, ("London", "Punch and Judy")},
    { 3, ("Toronto", "Rogers center") },
    { 4, ("Los Angeles", "Culver city") },
    { 5, ("Vancouver", "Stanley Park") },
    { 6, ("New York", "Central Park") },
    { 7, ("Sydney", "Harbour Bridge") },
    { 8, ("Quebec City", "Château Frontenac") },
    { 9, ("Paris", "The Louvre") },
    { 10, ("Lisbon", "Oceanário de Lisboa") },
  };

  private static async Task AddFollowers(DataContext dbContext, UserManager<AppUser> userManager) {
    if (dbContext.Followings.Any()) return;

    var users = userManager.Users.ToList();
    var followings = new List<UserFollowing> {
        new() { Observer = users[0], Target = users[2] },

        new() { Observer = users[1], Target = users[0] },
        new() { Observer = users[1], Target = users[2] },
        new() { Observer = users[1], Target = users[3] },
        new() { Observer = users[1], Target = users[4] },
        new() { Observer = users[1], Target = users[5] },

        new() { Observer = users[2], Target = users[0] },
        new() { Observer = users[2], Target = users[1] },
        new() { Observer = users[2], Target = users[6] },
        new() { Observer = users[2], Target = users[4] },
        new() { Observer = users[2], Target = users[5] },

        new() { Observer = users[3], Target = users[0] },
        new() { Observer = users[3], Target = users.Last() },
      };

    await dbContext.Followings.AddRangeAsync(followings);
    await dbContext.SaveChangesAsync();
  }

  private static async Task AddActivities(DataContext dbContext, UserManager<AppUser> userManager) {
    if (dbContext.Activities.Any()) return;

    var users = userManager.Users.ToList();

    for (var i = 10; i <= 100; ++i) {
      var date = DateTime.Now.AddDays(i - 95);
#pragma warning disable CA5394 // Do not use insecure randomness
      var cat = categories[rand.Next(1, categories.Count)];
      var (city, venue) = cities[rand.Next(1, cities.Count)];
      var usrs = users.OrderBy(x => Guid.NewGuid()).ToList();
#pragma warning restore CA5394 // Do not use insecure randomness

      await dbContext.Activities.AddAsync(new() {
        Title = $"Activity " + i,
        Date = date,
        Description = $"{cat.desc}",
        Category = cat.cat,
        City = city,
        Venue = venue,
        UserActivities = new List<UserActivity> {
          new() { AppUser = usrs[0], IsHost = true, DateJoined = date },
          new() { AppUser = usrs[1], IsHost = false, DateJoined = date.AddDays(-1) },
          new() { AppUser = usrs[2], IsHost = true, DateJoined = date.AddDays(-1) },
          new() { AppUser = usrs[3], IsHost = false, DateJoined = date.AddDays(-1) },
          new() { AppUser = usrs[4], IsHost = false, DateJoined = date.AddDays(-1) },
          new() { AppUser = usrs[5], IsHost = false, DateJoined = date.AddDays(-1) },
        },
        Comments = new List<Comment> {
          new() { Author = usrs[0], Body = "Still on?" },
          new() { Author = usrs[1], Body = "Of course it's on" },
          new() { Author = usrs[2], Body = "This is awesome" },
          new() { Author = usrs[3], Body = "I'm in" },
          new() { Author = usrs[1], Body = "Let's do it" },
          new() { Author = usrs[2], Body = "J'adore ca" },
          new() { Author = usrs[3], Body = "I'm from USA, btw." },
          new() { Author = usrs[5], Body = "Hello from MTL" },
        }
      });
    }

    await dbContext.SaveChangesAsync();
  }

  private static async Task AddUsers(DataContext dbContext, UserManager<AppUser> userManager) {
    if (dbContext.Users.Any()) return;

    var i = 0;
    var Id = () => (++i).ToString(CultureInfo.InvariantCulture);
    var baseUrl = "https://res.cloudinary.com/stankansas/image/upload";

    var users = new List<AppUser> {
      new() {
        Id = "admin",
        DisplayName = "Admin",
        UserName = "admin",
        Email = "admin@test.com",
        Bio = "A passionate software engineer | Dad",
        Photos = new Photo[] {
          new() {Id=Id(), IsMain = true, Url = $"{baseUrl}/v1667788631/lrumy0yoaoa0h9p8zo2y.jpg"},
          new() {Id=Id(), IsMain = false, Url = $"{baseUrl}/v1609120019/wdxbk5qjkettlxnvzmy5.jpg"},
          new() {Id=Id(), IsMain = false, Url = $"{baseUrl}/v1667788547/eq2cmgcgonykgabnyfsn.jpg"},
          new() {Id=Id(), IsMain = false, Url = $"{baseUrl}/v1667787939/nirhhmh1ob7eg7s6qznd.jpg"},
        }
      },
      new() {
        Id = "jane",
        DisplayName = "Jane",
        UserName = "jane",
        Email = "jane@test.com",
        Bio = "A passionate Photographer | Student",
        Photos = new Photo[] {
          new() {Id=Id(), IsMain = true, Url = $"{baseUrl}/v1609119965/yglovzkycojx7f0zafgh.jpg"},
          new() {Id=Id(), IsMain = false, Url = $"{baseUrl}/v1667788951/hqcv71nr4unfyaoefbld.jpg"},
        }
      },
      new() {
        Id = "nicki",
        DisplayName = "Nicki",
        UserName = "nicki",
        Email = "nicki@test.com",
        Bio = "A passionate LifeCoach | Student",
        Photos = new Photo[] {
          new() {Id=Id(), IsMain = true, Url = $"{baseUrl}/v1667789135/w77rweub0je89khwqg02.jpg"},
          new() {Id=Id(), IsMain = false, Url = $"{baseUrl}/v1667788076/b27beexlxamrmulijxae.jpg"},
        }
      },
      new() {
        Id = "roxane",
        DisplayName = "Roxanna Achaemenid Princess",
        UserName = "roxane",
        Email = "roxane@test.com",
        Photos = new Photo[] {
          new() {Id=Id(), IsMain = true, Url = $"{baseUrl}/v1667788474/rwldoiraowj8biyf5h1m.jpg"},
          new() {Id=Id(), IsMain = false, Url = $"{baseUrl}/v1667788547/eq2cmgcgonykgabnyfsn.jpg"},
        }
      },
      new() {
        Id = "bob",
        DisplayName = "Bob",
        UserName = "bob",
        Email = "bob@test.com",
        Photos = new Photo[] {
          new() {Id=Id(), IsMain = true, Url = $"{baseUrl}/v1609119885/pcykcdlnyjbckrobnzwb.jpg"},
        }
      },
      new() {
        Id = "tom",
        DisplayName = "Tom",
        UserName = "tom",
        Email = "tom@test.com",
        Photos = new Photo[] {
          new() {Id=Id(), IsMain = true, Url = $"{baseUrl}/v1609120019/wdxbk5qjkettlxnvzmy5.jpg"},
        }
      },
      new() {
        Id = "john",
        DisplayName = "John",
        UserName = "john",
        Email = "john@test.com",
        Photos = new Photo[] {
          new() {Id=Id(), IsMain = true, Url = $"{baseUrl}/v1667787772/gd5xnpxthhbuy7blw4d4.jpg"},
          new() {Id=Id(), IsMain = false, Url = $"{baseUrl}/v1609120019/wdxbk5qjkettlxnvzmy5.jpg"},
        }
      },
      new() {
        Id = "hardy",
        DisplayName = "Hardy",
        UserName = "Hardy",
        Email = "hardy@test.com",
        Photos = new Photo[] {
          new() {Id=Id(), IsMain = true, Url = $"{baseUrl}/v1609120019/wdxbk5qjkettlxnvzmy5.jpg"},
        }
      },
      new() {
        Id = "dan",
        DisplayName = "Dan B.",
        UserName = "dan",
        Email = "dan@test.com",
        Photos = new Photo[] {
          new() {Id=Id(), IsMain = true, Url = $"{baseUrl}/v1667787862/lowikqunihh8c7iqe57a.jpg"},
        }
      },
      new() {
        Id = "liam",
        DisplayName = "Liam",
        UserName = "liam",
        Email = "liam@test.com",
        Photos = new Photo[] {
          new() {Id=Id(), IsMain = true, Url = $"{baseUrl}/v1667787939/nirhhmh1ob7eg7s6qznd.jpg"},
        }
      },
      new() {
        Id = "test",
        DisplayName = "Test",
        UserName = "test",
        Email = "test@test.com",
      },
    };

    foreach (var user in users) {
      var result = await userManager.CreateAsync(user, user.UserName);//"Pa$$w0rd");
      if (!result.Succeeded) {
        throw new Exception("Faild seeding users");
      }
    }
  }
}