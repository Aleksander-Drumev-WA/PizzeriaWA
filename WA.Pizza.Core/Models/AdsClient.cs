using WA.Pizza.Core.ModelConfig;

namespace WA.Pizza.Core.Models
{
	public class AdsClient : BaseEntity
	{
		public string Name { get; set; } = null!;
		public Guid ApiKey { get; set; }
		public string? Website { get; set; }

		public List<Advertisement> Advertisements { get; set; } = new();
	}
}
