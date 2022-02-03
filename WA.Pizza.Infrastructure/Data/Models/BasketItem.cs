using System.Collections.Generic;

namespace WA.Pizza.Infrastructure.Data.Models
{
    public class BasketItem
    {
        public int Id { get; set; }

        // FK
        public int BasketId { get; set; }

        public int Quantity { get; set; }

        // FK
        public int CatalogItemId { get; set; }

    }
}
