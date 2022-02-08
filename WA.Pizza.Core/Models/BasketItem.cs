using System.Collections.Generic;
using WA.Pizza.Core.ModelConfig;

namespace WA.Pizza.Core.Models
{
    public class BasketItem : BaseEntity
    {
        public BasketItem()
        {
            this.OrderItems = new List<OrderItem>();
        }


        // FK
        public int BasketId { get; set; }

        public Basket Basket { get; set; }

        public int Quantity { get; set; }

        // FK
        public int CatalogItemId { get; set; }

        public CatalogItem CatalogItem { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
