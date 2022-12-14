using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity> {
  public void Configure(EntityTypeBuilder<UserActivity> builder) {
    //builder
    //  .HasIndex(ua => new { ua.AppUserId, ua.ActivityId })
    //  .IsUnique();

    builder.HasKey(ua => new { ua.AppUserId, ua.ActivityId });

    //Define relationship btw UserActivity & AppUser tables
    builder.HasOne(u => u.AppUser)
      .WithMany(a => a.UserActivities)
      .HasForeignKey(u => u.AppUserId)
      .OnDelete(DeleteBehavior.Restrict);

    //Define relationship btw UserActivity & Activity tables
    builder.HasOne(a => a.Activity)
      .WithMany(u => u.UserActivities)
      .HasForeignKey(a => a.ActivityId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}