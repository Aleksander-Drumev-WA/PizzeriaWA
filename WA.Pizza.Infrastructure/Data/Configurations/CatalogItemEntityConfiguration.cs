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
    internal class CatalogItemEntityConfiguration : IEntityTypeConfiguration<CatalogItem>
    {
        public void Configure(EntityTypeBuilder<CatalogItem> entity)
        {
            entity.HasKey(ci => ci.Id);

            entity
                .Property(ci => ci.Name)
                .HasColumnType("VARCHAR")
                .HasMaxLength(150)
                .IsRequired();

            // Range??
            entity
                .Property(ci => ci.Price)
                .IsRequired();

            entity
                .Property(ci => ci.PictureBytes)
                .HasColumnType("VARCHAR")
                .IsRequired();

            entity
                .HasMany(ci => ci.BasketItems)
                .WithOne(ci => ci.CatalogItem);
        }
    }
}
