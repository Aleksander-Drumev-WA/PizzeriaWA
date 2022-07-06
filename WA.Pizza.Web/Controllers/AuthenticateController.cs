using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WA.Pizza.Infrastructure.Data.Services;
using WA.Pizza.Infrastructure.DTO.Authentication;
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
		public Task<string> RegisterAdmin(RegisterRequest request)
		{
			return _authenticationDataService.RegisterAdmin(request);
		}

		[HttpPost("register")]
		public Task<string> Register(RegisterRequest request)
		{
			return _authenticationDataService.Register(request);
		}

		[HttpPost("login")]
		public Task<AuthResponseModel> Login(LoginRequest request)
		{
			return _authenticationDataService.Login(request);
		}

		[HttpPost("refreshToken")]
		public Task<AuthResponseModel> RefreshToken(TokenRequest tokenRequest)
		{
			return _authenticationDataService.VerifyToken(tokenRequest);
		}
	}
}
