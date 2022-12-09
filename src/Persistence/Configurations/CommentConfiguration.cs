using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment> {
  public void Configure(EntityTypeBuilder<Comment> builder) {
    builder.HasKey(ua => ua.Id);

    //builder.Property(t => t.Body)
    //  .IsRequired();

    builder
      .HasIndex(ua => new { ua.CreatedById, ua.ActivityId });
  }
}