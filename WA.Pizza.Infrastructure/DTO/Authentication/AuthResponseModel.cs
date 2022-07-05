namespace WA.Pizza.Infrastructure.DTO.Authentication
{
	public record AuthResponseModel
	{
		public AuthResponseModel()
		{
			Errors = new List<string>();
		}

		public string JwtToken { get; set; }

		public string RefreshToken { get; set; }

		public DateTime Expiration { get; set; }

		public List<string> Errors { get; set; }
	}
}
