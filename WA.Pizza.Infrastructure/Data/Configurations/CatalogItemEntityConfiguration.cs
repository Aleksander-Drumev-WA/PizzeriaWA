using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.Data.Configurations
{
    internal class CatalogItemEntityConfiguration : IEntityTypeConfiguration<CatalogItem>
    {
        public void Configure(EntityTypeBuilder<CatalogItem> entity)
        {
            entity.HasKey(ci => ci.Id);

            entity
                .Property(ci => ci.Name)
                .HasMaxLength(150)
                .IsRequired();

            // Range??
            entity
                .Property(ci => ci.Price)
                .IsRequired();

            entity
                .Property(ci => ci.PictureBytes)
                .HasMaxLength(30000)
                .IsRequired();

            entity
                .HasMany(ci => ci.BasketItems)
                .WithOne(ci => ci.CatalogItem);
        }
    }
}
