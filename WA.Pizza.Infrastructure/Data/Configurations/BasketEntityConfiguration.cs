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
    internal class BasketEntityConfiguration : IEntityTypeConfiguration<Basket>
    {
        public void Configure(EntityTypeBuilder<Basket> entity)
        {
            entity.HasKey(b => b.Id);

            entity.HasOne(b => b.User)
                .WithOne(b => b.Basket)
                .HasForeignKey<Basket>(b => b.UserId);
        }
    }
}
