using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Entities.Delivery
{
    public class DeliveryPartner : BaseAuditableEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // pochta, kuryer

        public string? Address { get; set; }
        public bool IsActive { get; set; } = true;

        // Service info
        public decimal DeliveryFee { get; set; } = 0;
        public string? ServiceAreas { get; set; } // JSON array of areas they serve
        public int EstimatedDeliveryDays { get; set; } = 1;

        // Statistics
        public int TotalDeliveries { get; set; } = 0;
        public int SuccessfulDeliveries { get; set; } = 0;
        public decimal AverageRating { get; set; } = 0;

        // Navigation Properties
        public virtual ICollection<OrderDelivery> OrderDeliveries { get; set; } = new List<OrderDelivery>();

        // Business Methods
        public decimal SuccessRate => TotalDeliveries > 0 ?
            (decimal)SuccessfulDeliveries / TotalDeliveries * 100 : 0;

        public void UpdateDeliveryStats(bool wasSuccessful)
        {
            TotalDeliveries++;
            if (wasSuccessful)
                SuccessfulDeliveries++;
        }
    }
}
