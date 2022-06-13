namespace WA.Pizza.Infrastructure.DTO.User
{
	public record LoginRequest
	{
		public string Email { get; set; }

		public string Password { get; set; }
	}
}
