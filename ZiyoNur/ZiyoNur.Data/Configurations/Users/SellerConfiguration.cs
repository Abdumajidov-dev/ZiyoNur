using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Users;

namespace ZiyoNur.Data.Configurations.Users;

public class SellerConfiguration : IEntityTypeConfiguration<Seller>
{
    public void Configure(EntityTypeBuilder<Seller> builder)
    {
        builder.ToTable("Sellers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Role)
            .HasMaxLength(50)
            .HasDefaultValue("seller");

        builder.Property(x => x.FcmToken)
            .HasMaxLength(500);

        builder.Property(x => x.TotalSalesAmount)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.TotalOrdersProcessed)
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(x => x.Phone)
            .IsUnique()
            .HasDatabaseName("IX_Sellers_Phone");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_Sellers_IsActive");

        // Ignore Domain Events and computed properties
        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.FullName);
    }
}
