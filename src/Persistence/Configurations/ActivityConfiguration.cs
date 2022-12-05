using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ActivityConfiguration : IEntityTypeConfiguration<Activity> {
  public void Configure(EntityTypeBuilder<Activity> builder) {
    builder.HasKey(ua => ua.Id);

    builder.Property(t => t.Title)
      .HasMaxLength(50)
      .IsRequired();
    builder.Property(t => t.Description)
      .HasMaxLength(500);
    builder.Property(t => t.Category)
      .HasMaxLength(50)
      .IsRequired();
    builder.Property(t => t.City)
      .HasMaxLength(50)
      .IsRequired();
    builder.Property(t => t.Venue)
      .HasMaxLength(50)
      .IsRequired();
  }
}