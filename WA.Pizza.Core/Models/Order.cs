using WA.Pizza.Core.ModelConfig;

namespace WA.Pizza.Core.Models
{
    public class Order : BaseEntity
    {
        public Order()
        {
            this.OrderItems = new List<OrderItem>();
        }

        public DateTime CreatedOn { get; set; }

        // FK
        public int UserId { get; set; }

        public User User { get; set; }

        public decimal Total { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
