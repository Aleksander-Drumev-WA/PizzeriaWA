namespace WA.Pizza.Infrastructure.DTO.Advertisement
{
	public record AdvertisementPutRequest
	{
		public int Id { get; set; }

		public string? Advertiser { get; set; }

		public string? AdvertiserUrl { get; set; }

		public string? PictureBytes { get; set; }

		public string? Title { get; set; }

		public string? Description { get; set; }

		public bool Failed { get; set; }
	}
}
