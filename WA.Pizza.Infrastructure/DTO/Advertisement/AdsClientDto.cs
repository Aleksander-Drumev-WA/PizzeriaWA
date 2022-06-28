namespace WA.Pizza.Infrastructure.DTO.Advertisement
{
	public record AdsClientDto
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public Guid ApiKey { get; set; }

		public string? Website { get; set; }

		public List<AdvertisementDTO> Advertisements { get; set; }
	}

}
