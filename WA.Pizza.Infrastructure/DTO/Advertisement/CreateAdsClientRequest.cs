using System.ComponentModel.DataAnnotations;

namespace WA.Pizza.Infrastructure.DTO.Advertisement
{
	public record CreateAdsClientRequest
	{
        [Required]
        [MaxLength(128)]
        public string Name { get; init; } = null!;

        [Url]
        [MaxLength(512)]
        public string? Website { get; init; }
    }
}
