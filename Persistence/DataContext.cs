using System;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class DataContext : IdentityDbContext<AppUser> {
    public DataContext(DbContextOptions options) : base(options) {
    }

    //public DbSet<Value> Values { get;set;}
    public DbSet<WeatherForecast> WeatherForecasts { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<UserActivity> UserActivities { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<UserFollowing> Followings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        //whenever running migration scripts it adds the followings to db => seed db
        modelBuilder.Entity<WeatherForecast>()
            .HasData(
            new { Id = 1, TemperatureC = -15, Date = DateTime.Now, Summary = "Freezing" },
            new { Id = 2, TemperatureC = 16, Date = DateTime.Now.AddDays(1), Summary = "Chilly" },
            new { Id = 3, TemperatureC = 20, Date = DateTime.Now.AddDays(2), Summary = "Cool" },
            new { Id = 4, TemperatureC = 25, Date = DateTime.Now.AddDays(3), Summary = "Mild" },
            new { Id = 5, TemperatureC = 30, Date = DateTime.Now.AddDays(4), Summary = "Warm" },
            new { Id = 6, TemperatureC = 40, Date = DateTime.Now.AddDays(5), Summary = "Hot" },
            new { Id = 7, TemperatureC = 45, Date = DateTime.Now.AddDays(6), Summary = "Scorching" }
            );

        //Define Primary Keys for UserActivity
        modelBuilder.Entity<UserActivity>(entity => {
            entity.HasKey(ua => new { ua.AppUserId, ua.ActivityId });

            //Define relationship btw UserActivity & AppUser tables
            entity.HasOne(u => u.AppUser)
              .WithMany(a => a.UserActivities)
              .HasForeignKey(u => u.AppUserId);

            //Define relationship btw UserActivity & Activity tables
            entity.HasOne(a => a.Activity)
                .WithMany(u => u.UserActivities)
                .HasForeignKey(a => a.ActivityId);
        });

        //ef fluent configuration
        modelBuilder.Entity<UserFollowing>(entity => {
            entity.HasKey(k => new { k.ObserverId, k.TargetId });

            //define many-to-many relationship

            entity.HasOne(o => o.Observer)
                .WithMany(f => f.Followings)
                .HasForeignKey(o => o.ObserverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Target)
                .WithMany(f => f.Followers)
                .HasForeignKey(o => o.TargetId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

