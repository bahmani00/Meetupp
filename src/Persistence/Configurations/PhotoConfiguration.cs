using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo> {
  public void Configure(EntityTypeBuilder<Photo> builder) {
    builder.HasKey(ua => ua.Id);

    builder.Property(t => t.PublicId)
      .HasMaxLength(500)
      .IsRequired();

    builder.Property(t => t.Url)
      .IsRequired();

    builder.Property(t => t.IsMain);
  }
}