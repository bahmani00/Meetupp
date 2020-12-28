using System;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        //public DbSet<Value> Values { get;set;}
        public DbSet<WeatherForecast> WeatherForecasts { get;set;}
        public DbSet<Activity> Activities { get;set;}
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //whenever running migration scripts it adds the followings to db => seed db
            modelBuilder.Entity<WeatherForecast>()
                .HasData(
                new WeatherForecast { Id = 1, TemperatureC = -15, Date = DateTime.Now, Summary = "Freezing" },
                new WeatherForecast { Id = 2, TemperatureC = 16, Date = DateTime.Now.AddDays(1), Summary = "Chilly" },
                new WeatherForecast { Id = 3, TemperatureC = 20, Date = DateTime.Now.AddDays(2), Summary = "Cool" },
                new WeatherForecast { Id = 4, TemperatureC = 25, Date = DateTime.Now.AddDays(3), Summary = "Mild" },
                new WeatherForecast { Id = 5, TemperatureC = 30, Date = DateTime.Now.AddDays(4), Summary = "Warm" },
                new WeatherForecast { Id = 6, TemperatureC = 40, Date = DateTime.Now.AddDays(5), Summary = "Hot" },
                new WeatherForecast { Id = 7, TemperatureC = 45, Date = DateTime.Now.AddDays(6), Summary = "Scorching" }
                );

            //Define Primary Keys for UserActivity
            modelBuilder.Entity<UserActivity>(x => x.HasKey(ua =>
                new { ua.AppUserId, ua.ActivityId }));

            //Define relationship btw UserActivity & AppUser tables
            modelBuilder.Entity<UserActivity>()
                .HasOne(u => u.AppUser)
                .WithMany(a => a.UserActivities)
                .HasForeignKey(u => u.AppUserId);

            //Define relationship btw UserActivity & Activity tables
            modelBuilder.Entity<UserActivity>()
                .HasOne(a => a.Activity)
                .WithMany(u => u.UserActivities)
                .HasForeignKey(a => a.ActivityId);
        }
    }
}

