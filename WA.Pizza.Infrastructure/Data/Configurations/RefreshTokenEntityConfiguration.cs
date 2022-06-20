using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.Data.Configurations
{
	internal class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshToken>
	{
		public void Configure(EntityTypeBuilder<RefreshToken> entity)
		{
			entity
				.Property(x => x.Token)
				.HasMaxLength(4000)
				.IsRequired();

			entity
				.Property(x => x.JwtId)
				.HasMaxLength(200)
				.IsRequired();

			entity
				.Property(x => x.IsUsed)
				.IsRequired();

			entity
				.Property(x => x.IsRevoked)
				.IsRequired();

			entity
				.Property(x => x.LastModifiedOn)
				.IsRequired(false);

			entity
				.Property(x => x.ExpiryDate)
				.IsRequired();

			entity
				.HasOne(x => x.User)
				.WithOne(x => x.RefreshToken);
		}
	}
}
