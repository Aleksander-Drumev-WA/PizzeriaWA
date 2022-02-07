using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.Data.Configurations
{

    internal class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.HasOne(u => u.Basket)
                .WithOne(u => u.User);

            entity
                .HasMany(u => u.Orders)
                .WithOne(u => u.User);
        }
    }
}
