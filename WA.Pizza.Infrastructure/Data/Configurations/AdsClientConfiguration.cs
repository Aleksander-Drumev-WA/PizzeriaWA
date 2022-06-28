using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.Data.Configurations
{
	internal class AdsClientConfiguration : IEntityTypeConfiguration<AdsClient>
	{
		public void Configure(EntityTypeBuilder<AdsClient> entity)
		{
			entity
				.Property(x => x.Name)
				.HasMaxLength(128)
				.IsRequired();

			entity
				.Property(x => x.Website)
				.HasMaxLength(512)
				.IsRequired(false);

			entity.HasIndex(ac => ac.ApiKey)
				  .IsUnique();
		}
	}
}
