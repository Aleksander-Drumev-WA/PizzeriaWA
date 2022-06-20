using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.DTO.User;
using WA.Pizza.Infrastructure.DTO.Authentication;

using static WA.Pizza.Core.ConstantValues;
using Microsoft.EntityFrameworkCore;

namespace WA.Pizza.Infrastructure.Data.Services
{
	public class AuthenticationDataService
	{
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<Role> _roleManager;
		private readonly IConfiguration _configuration;
		private readonly AppDbContext _dbContext;
		private readonly TokenValidationParameters _tokenValidationParams;

		public AuthenticationDataService(
			UserManager<User> userManager,
			RoleManager<Role> roleManager,
			IConfiguration configuration,
			AppDbContext dbContext,
			TokenValidationParameters tokenValidationParams)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_configuration = configuration;
			_dbContext = dbContext;
			_tokenValidationParams = tokenValidationParams;
		}

		public async Task<string> Register(RegisterRequest request)
		{
			var userExists = await _userManager.FindByNameAsync(request.UserName);
			if (userExists != null)
			{
				return "User already exists.";

			}

			User user = new User()
			{
				Email = request.Email,
				SecurityStamp = Guid.NewGuid().ToString(),
				UserName = request.UserName
			};
			var result = await _userManager.CreateAsync(user, request.Password);
			if (!result.Succeeded)
			{
				return "User creation failed! Please check user details and try again.";

			}

			return user.Id.ToString();
		}

		public async Task<AuthResponseModel> Login(LoginRequest request)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);
			var authCredentials = new AuthResponseModel();
			if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
			{
				var authClaims = await GenerateClaims(user);

				authCredentials = await GenerateJwtToken(user, authClaims);
			}
			else
			{
				authCredentials.Errors.Add("User does not exists or invalid password input");
			}

			return authCredentials;
		}

		public async Task<string> RegisterAdmin(RegisterRequest request)
		{
			var userExists = await _userManager.FindByEmailAsync(request.Email);
			if (userExists != null)
			{
				return "User already exists!";
			}

			User user = new User()
			{
				Email = request.Email,
				SecurityStamp = Guid.NewGuid().ToString(),
				UserName = request.UserName
			};
			var result = await _userManager.CreateAsync(user, request.Password);
			if (!result.Succeeded)
			{
				return "User creation failed! Please check user details and try again.";
			}

			if (!await _roleManager.RoleExistsAsync(UserRoles.ADMIN_ROLE_NAME))
				await _roleManager.CreateAsync(new Role(UserRoles.ADMIN_ROLE_NAME));
			if (!await _roleManager.RoleExistsAsync(UserRoles.REGULAR_USER_ROLE_NAME))
				await _roleManager.CreateAsync(new Role(UserRoles.REGULAR_USER_ROLE_NAME));

			if (await _roleManager.RoleExistsAsync(UserRoles.ADMIN_ROLE_NAME))
			{
				await _userManager.AddToRoleAsync(user, UserRoles.ADMIN_ROLE_NAME);
			}

			return user.Id.ToString();
		}

		public async Task<AuthResponseModel> VerifyToken(TokenRequest tokenRequest)
		{
			var jwtTokenHandler = new JwtSecurityTokenHandler();
			var authResponse = new AuthResponseModel();

			// Validation 1
			var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParams, out var validatedToken);

			// Validation 2
			if (validatedToken is JwtSecurityToken jwtSecurityToken)
			{
				var isRightAlgoritmUsed = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
				if (!isRightAlgoritmUsed)
				{
					return null;
				}
			}

			// Validation 3
			var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!.Value);

			var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

			if (expiryDate > DateTime.UtcNow)
			{
				authResponse.Errors.Add("Token has not expired yet.");

				return authResponse;
			}

			//Validation 4
			var storedToken = await _dbContext
				.RefreshTokens
				.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

			if (storedToken == null)
			{
				authResponse.Errors.Add("Token does not exist.");

				return authResponse;
			}

			// Validation 5
			if (storedToken.IsUsed)
			{
				authResponse.Errors.Add("Token has been used.");

				return authResponse;
			}

			// Validation 6
			if (storedToken.IsRevoked)
			{
				authResponse.Errors.Add("Token has been revoked.");

				return authResponse;
			}

			// Validation 7
			var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)!.Value;

			if (storedToken.JwtId != jti)
			{
				authResponse.Errors.Add("Token does not match.");

				return authResponse;
			}

			// update current token
			storedToken.IsUsed = true;

			// create new one
			var dbUser = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
			var authClaims = await GenerateClaims(dbUser);
			return await GenerateJwtToken(dbUser, authClaims);
		}

		private async Task<AuthResponseModel> GenerateJwtToken(User user, IEnumerable<Claim> authClaims)
		{
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

			var token = new JwtSecurityToken(
						issuer: _configuration["JWT:ValidIssuer"],
						audience: _configuration["JWT:ValidAudience"],
						expires: DateTime.Now.AddSeconds(30),
						claims: authClaims,
						signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

			var refreshToken = new RefreshToken()
			{
				JwtId = token.Id,
				IsUsed = false,
				IsRevoked = false,
				UserId = user.Id,
				LastModifiedOn = DateTime.UtcNow,
				ExpiryDate = DateTime.UtcNow.AddMonths(6),
				Token = RandomString(35) + Guid.NewGuid().ToString()
			};

			var storedRefreshToken = await _dbContext
				.RefreshTokens
				.FirstOrDefaultAsync(x => x.UserId == user.Id);

			if (storedRefreshToken == null)
			{
				_dbContext.RefreshTokens.Add(refreshToken);
			}
			else
			{
				storedRefreshToken = refreshToken;
				_dbContext.RefreshTokens.Update(storedRefreshToken);
			}

			await _dbContext.SaveChangesAsync();

			var result = new AuthResponseModel()
			{
				JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
				Expiration = token.ValidTo,
				RefreshToken = storedRefreshToken!.Token
			};

			return result;
		}

		private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
		{
			var dateTimeValue = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

			dateTimeValue = dateTimeValue.AddSeconds(unixTimeStamp).ToLocalTime();
			return dateTimeValue;
		}

		private string RandomString(int length)
		{
			var random = new Random();

			var availableCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";

			return new string(Enumerable.Repeat(availableCharacters, length)
				.Select(x => x[random.Next(x.Length)]).ToArray());
		}

		private async Task<IEnumerable<Claim>> GenerateClaims(User user)
		{
			var authClaims = new List<Claim>
			{
				new Claim("Id", user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(ClaimTypes.Name, user.UserName),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};

			var userRoles = await _userManager.GetRolesAsync(user);

			foreach (var userRole in userRoles)
			{
				authClaims.Add(new Claim(ClaimTypes.Role, userRole));
			}

			return authClaims;
		}
	}
}
