using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WA.Pizza.Core.Models;

using static WA.Pizza.Core.ConstantValues;

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

            entity
                .Property(x => x.UserName)
                .HasMaxLength(Validations.MAXIMUM_USERNAME_CHARACTERS)
                .IsRequired();

            entity
                .Property(x => x.NormalizedUserName)
                .HasMaxLength(Validations.MAXIMUM_USERNAME_CHARACTERS)
                .IsRequired();

            entity
                .Property(x => x.Email)
                .IsRequired();

            entity.Property(x => x.NormalizedEmail)
                .IsRequired();
        }
    }
}
