using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Entities.Products
{
    public class ProductPriceChangedEvent : BaseEvent
    {
        public int ProductId { get; }
        public decimal OldPrice { get; }
        public decimal NewPrice { get; }

        public ProductPriceChangedEvent(int productId, decimal oldPrice, decimal newPrice)
        {
            ProductId = productId;
            OldPrice = oldPrice;
            NewPrice = newPrice;
        }
    }
}
