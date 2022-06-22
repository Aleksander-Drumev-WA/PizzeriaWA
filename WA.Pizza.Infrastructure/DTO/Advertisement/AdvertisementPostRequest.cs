namespace WA.Pizza.Infrastructure.DTO.Advertisement
{
	public record AdvertisementPostRequest
	{
		public string Advertiser { get; set; }

		public string AdvertiserUrl { get; set; }

		public string PictureBytes { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }
	}
}
