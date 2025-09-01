using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Users;

namespace ZiyoNur.Data.Configurations.Users;

public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.ToTable("Admins");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Role)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("admin");

        builder.Property(x => x.Phone)
            .HasMaxLength(20);

        builder.Property(x => x.Email)
            .HasMaxLength(100);

        builder.Property(x => x.FcmToken)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(x => x.Username)
            .IsUnique()
            .HasDatabaseName("IX_Admins_Username");

        builder.HasIndex(x => x.Phone)
            .IsUnique()
            .HasDatabaseName("IX_Admins_Phone")
            .HasFilter("[Phone] IS NOT NULL");

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("IX_Admins_Email")
            .HasFilter("[Email] IS NOT NULL");

        // Ignore Domain Events and computed properties
        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.FullName);
    }
}
