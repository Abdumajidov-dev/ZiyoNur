using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Users;

namespace ZiyoNur.Data.Configurations.Users;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

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

        builder.Property(x => x.Email)
            .HasMaxLength(100);

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Address)
            .HasMaxLength(500);

        builder.Property(x => x.FcmToken)
            .HasMaxLength(500);

        builder.Property(x => x.TotalCashback)
            .HasPrecision(18, 2);

        // Indexes
        builder.HasIndex(x => x.Phone)
            .IsUnique()
            .HasDatabaseName("IX_Customers_Phone");

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("IX_Customers_Email")
            .HasFilter("[Email] IS NOT NULL");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_Customers_CreatedAt");

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.FullName);
    }
}
