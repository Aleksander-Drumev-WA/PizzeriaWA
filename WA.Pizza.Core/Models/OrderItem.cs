using WA.Pizza.Core.ModelConfig;

namespace WA.Pizza.Core.Models
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }

        public Order Order { get; set; }

        public int? CatalogItemId { get; set; }

        public CatalogItem CatalogItem { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
