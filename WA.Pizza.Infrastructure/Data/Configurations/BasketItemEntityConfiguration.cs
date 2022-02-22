using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WA.Pizza.Core.Models;

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

            entity
                .Property(bi => bi.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity
                .Property(bi => bi.Price)
                .IsRequired();

            entity.HasOne(bi => bi.CatalogItem)
                .WithMany(bi => bi.BasketItems)
                .HasForeignKey(bi => bi.CatalogItemId);
        }
    }
}
