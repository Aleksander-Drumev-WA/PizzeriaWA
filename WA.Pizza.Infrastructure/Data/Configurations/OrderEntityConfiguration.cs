using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WA.Pizza.Infrastructure.Data.Models;

namespace WA.Pizza.Infrastructure.Data.Configurations
{
    internal class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> entity)
        {
            entity.HasKey(o => o.Id);

            entity
                .Property(o => o.CreatedOn)
                .HasColumnType("DATETIME2")
                .IsRequired();

            entity
                .HasOne(o => o.User)
                .WithMany(o => o.Orders);

            entity
                .Property(o => o.Total)
                .IsRequired();
        }
    }
}
