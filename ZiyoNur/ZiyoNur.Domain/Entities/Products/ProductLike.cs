using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Users;

namespace ZiyoNur.Domain.Entities.Products;

public class ProductLike : BaseAuditableEntity
{
    public int CustomerId { get; set; }
    public int ProductId { get; set; }

    // Navigation Properties
    public virtual Customer Customer { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
