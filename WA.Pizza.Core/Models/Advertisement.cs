using WA.Pizza.Core.ModelConfig;

namespace WA.Pizza.Core.Models
{
	public class Advertisement : BaseEntity
	{
		public string PictureBytes { get; set; } = null!;

		public string Title { get; set; } = null!;

		public string? Description { get; set; } = null!;

		public int AdsClientId { get; set; }

		public AdsClient? AdsClient { get; set; }
	}
}
