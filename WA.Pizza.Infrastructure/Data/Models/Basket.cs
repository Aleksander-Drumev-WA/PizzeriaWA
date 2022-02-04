using System.Collections.Generic;

namespace WA.Pizza.Infrastructure.Data.Models
{
    public class Basket
    {
        public Basket()
        {
            BasketItems = new HashSet<BasketItem>();
        }

        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public ICollection<BasketItem> BasketItems { get; set; }
    }
}
