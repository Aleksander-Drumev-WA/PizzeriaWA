namespace WA.Pizza.Infrastructure.DTO.Advertisement
{
	public record AdsClientGridDto
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public Guid ApiKey { get; set; }

		public string? Website { get; set; }
	}
}
