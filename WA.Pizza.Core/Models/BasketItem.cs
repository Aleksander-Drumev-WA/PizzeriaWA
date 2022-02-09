using System.Collections.Generic;
using WA.Pizza.Core.ModelConfig;

namespace WA.Pizza.Core.Models
{
    public class BasketItem : BaseEntity
    {
        // FK
        public int BasketId { get; set; }

        public Basket Basket { get; set; }

        public int Quantity { get; set; }

        // FK
        public int CatalogItemId { get; set; }

        public CatalogItem CatalogItem { get; set; }
    }
}
