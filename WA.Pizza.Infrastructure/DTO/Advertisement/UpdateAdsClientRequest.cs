using System.ComponentModel.DataAnnotations;

namespace WA.Pizza.Infrastructure.DTO.Advertisement
{
	public record UpdateAdsClientRequest
	{
		public int Id { get; set; }

		[Required]
        [MaxLength(128)]
        public string Name { get; init; } = null!;

        [Required]
        public Guid ApiKey { get; init; }

        [MaxLength(512)]
        public string Website { get; init; } = null!;
    }
}
