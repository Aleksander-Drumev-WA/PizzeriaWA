using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Infrastructure.Data.Models;

namespace WA.Pizza.Infrastructure.Data.Configurations
{
    internal class BasketItemEntityConfiguration : IEntityTypeConfiguration<BasketItem>
    {
        public void Configure(EntityTypeBuilder<BasketItem> entity)
        {
            entity.HasKey(bi => bi.Id);

            entity
                .HasOne(bi => bi.Basket)
                .WithMany(bi => bi.BasketItems)
                .HasForeignKey(bi => bi.BasketId);

            entity
                .Property(bi => bi.Quantity)
                .IsRequired();

            entity.HasOne(bi => bi.CatalogItem)
                .WithMany(bi => bi.BasketItems)
                .HasForeignKey(bi => bi.CatalogItemId);
        }
    }
}
