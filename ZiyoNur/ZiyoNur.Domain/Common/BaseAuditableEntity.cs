using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZiyoNur.Domain.Common
{
    public abstract class BaseAuditableEntity : BaseEntity
    {
        public int? CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public int? DeletedById { get; set; }

        public string? CreatedByType { get; set; } // admin, seller, customer, system
        public string? UpdatedByType { get; set; }
        public string? DeletedByType { get; set; }
    }
}
