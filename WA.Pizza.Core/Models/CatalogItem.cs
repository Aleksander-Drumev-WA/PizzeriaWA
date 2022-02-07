using System.Collections.Generic;
using WA.Pizza.Core.ModelConfig;

namespace WA.Pizza.Core.Models
{
    public class CatalogItem : BaseEntity
    {
        public CatalogItem()
        {
            BasketItems = new HashSet<BasketItem>();
        }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string PictureBytes { get; set; }

        public ICollection<BasketItem> BasketItems { get; set; }
    }
}
