namespace WA.Pizza.Infrastructure.DTO.Authentication
{
	public record TokenRequest
	{
		public string Token { get; set; }

		public string RefreshToken { get; set; }
	}
}
