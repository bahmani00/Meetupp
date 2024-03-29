using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo> {
  public void Configure(EntityTypeBuilder<Photo> builder) {
    builder.HasKey(ua => ua.Id);

    builder.Property(t => t.PublicId)
      .HasMaxLength(450);

    builder.Property(t => t.IsMain);
  }
}