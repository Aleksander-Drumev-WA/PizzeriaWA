using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User, Role, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public DbSet<User> Users { get; set; }

		public DbSet<Role> Roles { get; set; }

		public DbSet<Basket> Baskets { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<CatalogItem> CatalogItems { get; set; }

        public DbSet<BasketItem> BasketItems { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

		public DbSet<RefreshToken> RefreshTokens { get; set; }

		public DbSet<Advertisement> Advertisements { get; set; }
	}
}
