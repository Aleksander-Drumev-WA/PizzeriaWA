using WA.Pizza.Core.ModelConfig;

namespace WA.Pizza.Core.Models
{
    public class Order : BaseEntity
    {
        public DateTime CreatedOn { get; set; }

        // FK
        public int UserId { get; set; }

        public User User { get; set; }

        public decimal Total { get; set; }
    }
}
