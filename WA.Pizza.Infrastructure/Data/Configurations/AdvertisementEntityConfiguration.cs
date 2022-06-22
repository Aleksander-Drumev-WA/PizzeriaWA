using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.Data.Configurations
{
	internal class AdvertisementEntityConfiguration : IEntityTypeConfiguration<Advertisement>
	{
		public void Configure(EntityTypeBuilder<Advertisement> entity)
		{
			entity
				.Property(x => x.Advertiser)
				.HasMaxLength(150)
				.IsRequired();

			entity
				.Property(x => x.AdvertiserUrl)
				.HasMaxLength(150)
				.IsRequired();

			entity
				.Property(x => x.PictureBytes)
				.HasMaxLength(30000)
				.IsRequired();

			entity
				.Property(x => x.Title)
				.HasMaxLength(150)
				.IsRequired();

			entity
				.Property(x => x.Description)
				.HasMaxLength(500)
				.IsRequired();
		}
	}
}
