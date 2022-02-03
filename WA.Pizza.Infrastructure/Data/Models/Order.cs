using System.Collections.Generic;

namespace WA.Pizza.Infrastructure.Data.Models
{
    public class Order
    {
        public int Id { get; set; }

        // BaseModel??
        public DateTime CreatedOn { get; set; }

        // FK
        public int UserId { get; set; }

        public decimal Total { get; set; }
    }
}
