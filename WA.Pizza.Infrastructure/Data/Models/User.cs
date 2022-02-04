using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace WA.Pizza.Infrastructure.Data.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            Orders = new HashSet<Order>();
        }

        public Basket Basket { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
