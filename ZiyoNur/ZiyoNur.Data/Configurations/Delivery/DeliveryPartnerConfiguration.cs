using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZiyoNur.Domain.Entities.Delivery;

namespace ZiyoNur.Data.Configurations.Delivery
{
    public class DeliveryPartnerConfiguration : IEntityTypeConfiguration<DeliveryPartner>
    {
        public void Configure(EntityTypeBuilder<DeliveryPartner> builder)
        {
            builder.ToTable("DeliveryPartners");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Address)
                .HasMaxLength(500);

            builder.Property(x => x.DeliveryFee)
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(x => x.ServiceAreas)
                .HasMaxLength(2000);

            builder.Property(x => x.EstimatedDeliveryDays)
                .HasDefaultValue(1);

            builder.Property(x => x.TotalDeliveries)
                .HasDefaultValue(0);

            builder.Property(x => x.SuccessfulDeliveries)
                .HasDefaultValue(0);

            builder.Property(x => x.AverageRating)
                .HasPrecision(3, 2)
                .HasDefaultValue(0);

            // Indexes
            builder.HasIndex(x => x.IsActive)
                .HasDatabaseName("IX_DeliveryPartners_IsActive");

            builder.HasIndex(x => x.Type)
                .HasDatabaseName("IX_DeliveryPartners_Type");

            builder.HasIndex(x => x.Phone)
                .HasDatabaseName("IX_DeliveryPartners_Phone");

            // Ignore computed properties
            builder.Ignore(x => x.SuccessRate);
        }
    }
}
