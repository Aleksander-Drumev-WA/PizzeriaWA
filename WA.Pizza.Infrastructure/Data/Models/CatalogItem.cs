using System.Collections.Generic;

namespace WA.Pizza.Infrastructure.Data.Models
{
    public class CatalogItem
    {
        public int Id { get; set; }

        // Shows warning and wants to be nullable.
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string PictureBytes { get; set; }
    }
}
