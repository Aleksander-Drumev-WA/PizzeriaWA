namespace WA.Pizza.Infrastructure.DTO.Advertisement
{
	public record AdvertisementDTO
	{
		public int Id { get; set; }

		public string PictureBytes { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }
	}
}
