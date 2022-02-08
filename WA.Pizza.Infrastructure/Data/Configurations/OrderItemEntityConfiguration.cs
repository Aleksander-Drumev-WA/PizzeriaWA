using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.Data.Configurations
{
    internal class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> entity)
        {
            entity.HasKey(oi => new { oi.OrderId, oi.BasketItemId});

            entity
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems);

            entity
                .HasOne(oi => oi.BasketItem)
                .WithMany(bi => bi.OrderItems);
                
        }
    }
}
