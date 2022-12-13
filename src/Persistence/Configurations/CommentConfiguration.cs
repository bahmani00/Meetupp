using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment> {
  public void Configure(EntityTypeBuilder<Comment> builder) {
    builder
      .ToTable("Comment")
      .HasKey(ua => ua.Id);

    //builder.Property(t => t.Body)
    //  .IsRequired();

    //builder
    //  .HasIndex(ua => new { ua.CreatedById, ua.ActivityId })
    //  .IsUnique();


    //Define relationship btw UserActivity & AppUser tables
    builder.HasOne(c => c.CreatedBy)
    .WithMany()
    .OnDelete(DeleteBehavior.Restrict);

    //Define relationship btw UserActivity & Activity tables
    builder.HasOne(a => a.Activity)
      .WithMany(u => u.Comments)
      .HasForeignKey(a => a.ActivityId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}