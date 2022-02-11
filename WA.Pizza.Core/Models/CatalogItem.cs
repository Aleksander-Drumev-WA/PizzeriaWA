using System.Collections.Generic;
using WA.Pizza.Core.ModelConfig;

namespace WA.Pizza.Core.Models
{
    public class CatalogItem : BaseEntity
    {
        public CatalogItem()
        {
            BasketItems = new HashSet<BasketItem>();
            OrderItems = new HashSet<OrderItem>();
        }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string PictureBytes { get; set; }

        public int StorageQuantity { get; set; }

        public ICollection<BasketItem> BasketItems { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
