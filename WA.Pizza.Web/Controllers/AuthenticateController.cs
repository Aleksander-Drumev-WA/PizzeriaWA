using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WA.Pizza.Infrastructure.Data.Services;
using WA.Pizza.Infrastructure.DTO.User;

namespace WA.Pizza.Web.Controllers
{
	[AllowAnonymous]
	public class AuthenticateController : BaseController
	{
		private readonly AuthenticationDataService _authenticationDataService;

		public AuthenticateController(AuthenticationDataService authenticationDataService)
		{
			_authenticationDataService = authenticationDataService;
		}

		[HttpPost("register-admin")]
		public async Task<string> RegisterAdmin(RegisterRequest request)
		{
			return await _authenticationDataService.RegisterAdmin(request);
		}

		[HttpPost("register")]
		public async Task<string> Register(RegisterRequest request)
		{
			return await _authenticationDataService.Register(request);
		}

		[HttpPost("login")]
		public async Task<object> Login(LoginRequest request)
		{
			return await _authenticationDataService.Login(request);
		}
	}
}
