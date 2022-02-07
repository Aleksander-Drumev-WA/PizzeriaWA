using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.Data.Configurations
{
    internal class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> entity)
        {
            entity.HasKey(o => o.Id);

            entity
                .Property(o => o.CreatedOn)
                .IsRequired();

            entity
                .HasOne(o => o.User)
                .WithMany(o => o.Orders)
                .HasForeignKey(o => o.UserId);

            entity
                .Property(o => o.Total)
                .IsRequired();
        }
    }
}
