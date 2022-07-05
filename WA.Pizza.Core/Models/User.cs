using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace WA.Pizza.Core.Models
{
    public class User : IdentityUser<int>
    {
        public User()
        {
            Orders = new HashSet<Order>();
        }

        public Basket Basket { get; set; }

		public RefreshToken RefreshToken { get; set; }

		public ICollection<Order> Orders { get; set; }
    }
}
