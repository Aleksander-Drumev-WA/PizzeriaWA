using System.ComponentModel.DataAnnotations;

namespace WA.Pizza.Infrastructure.DTO.Advertisement
{
	public record CreateAdvertisementRequest
	{
		[Required]
		[MaxLength(30000)]
		public string PictureBytes { get; set; }

		[Required]
		[MaxLength(150)]
		public string Title { get; set; }

		[Required]
		[MaxLength(512)]
		public string Description { get; set; }
	}
}
