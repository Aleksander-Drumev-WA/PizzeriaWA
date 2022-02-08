using WA.Pizza.Core.ModelConfig;

namespace WA.Pizza.Core.Models
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }

        public Order Order { get; set; }

        public int BasketItemId { get; set; }

        public BasketItem BasketItem { get; set; }
    }
}
