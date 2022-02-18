using System.Collections.Generic;
using WA.Pizza.Core.ModelConfig;

namespace WA.Pizza.Core.Models
{
    public class BasketItem : BaseEntity
    {
        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public int BasketId { get; set; }

        public Basket Basket { get; set; }

        public int CatalogItemId { get; set; }

        public CatalogItem CatalogItem { get; set; }
    }
}
