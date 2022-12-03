using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class UserFollowingConfiguration : IEntityTypeConfiguration<UserFollowing> {
  public void Configure(EntityTypeBuilder<UserFollowing> builder) {

    //builder
    //  .HasIndex(k => new { k.ObserverId, k.TargetId })
    //  .IsUnique();

    builder
      .HasKey(k => new { k.ObserverId, k.TargetId });

    //define many-to-many relationship
    builder
      .HasOne(o => o.Observer)
      .WithMany(f => f.Followings)
      .HasForeignKey(o => o.ObserverId)
      .OnDelete(DeleteBehavior.Restrict);

    builder
      .HasOne(o => o.Target)
      .WithMany(f => f.Followers)
      .HasForeignKey(o => o.TargetId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}