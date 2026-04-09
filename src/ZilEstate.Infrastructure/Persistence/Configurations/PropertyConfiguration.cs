using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZilEstate.Domain.Entities;

namespace ZilEstate.Infrastructure.Persistence.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Title).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(5000).IsRequired();
        builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
        builder.Property(p => p.SellerName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.SellerPhone).HasMaxLength(20).IsRequired();
        builder.Property(p => p.SellerWhatsApp).HasMaxLength(20);
        builder.Property(p => p.SellerEmail).HasMaxLength(200);

        builder.HasOne(p => p.Location)
            .WithMany(l => l.Properties)
            .HasForeignKey(p => p.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Images)
            .WithOne(i => i.Property)
            .HasForeignKey(i => i.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.Type);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.LocationId);
        builder.HasIndex(p => p.IsApproved);
        builder.HasIndex(p => p.CreatedAt);
    }
}
