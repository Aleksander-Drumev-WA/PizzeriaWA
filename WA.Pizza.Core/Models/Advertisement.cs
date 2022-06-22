using WA.Pizza.Core.ModelConfig;

namespace WA.Pizza.Core.Models
{
	public class Advertisement : BaseEntity
	{
		public string Advertiser { get; set; }

		public string AdvertiserUrl { get; set; }

		public string PictureBytes { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }
	}
}
