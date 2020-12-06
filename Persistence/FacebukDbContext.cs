using System;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace Persistence
{
    public class FacebukDbContext: DbContext
    {
        public FacebukDbContext(DbContextOptions options): base(options){

        }

        //public DbSet<Value> Values { get;set;}
        public DbSet<WeatherForecast> WeatherForecasts { get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
        }

    }
}

