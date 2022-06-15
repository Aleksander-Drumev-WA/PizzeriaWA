using System.Collections.Generic;
using WA.Pizza.Core.ModelConfig;

namespace WA.Pizza.Core.Models
{
    public class Basket : BaseEntity
    {
        public Basket()
        {
            BasketItems = new HashSet<BasketItem>();
        }
        public int? UserId { get; set; }

        public User User { get; set; }

		public DateTime? LastModifiedOn { get; set; }

		public ICollection<BasketItem> BasketItems { get; set; }
    }
}
