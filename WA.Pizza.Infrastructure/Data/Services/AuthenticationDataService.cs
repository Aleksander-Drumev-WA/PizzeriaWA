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

using static WA.Pizza.Core.ConstantValues;

namespace WA.Pizza.Infrastructure.Data.Services
{
	public class AuthenticationDataService
	{
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<Role> _roleManager;
		private readonly IConfiguration _configuration;

		public AuthenticationDataService(UserManager<User> userManager, RoleManager<Role> roleManager, IConfiguration configuration)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_configuration = configuration;
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

		public async Task<object> Login(LoginRequest request)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
			{
				var userRoles = await _userManager.GetRolesAsync(user);

				var authClaims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, user.UserName),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				};

				foreach (var userRole in userRoles)
				{
					authClaims.Add(new Claim(ClaimTypes.Role, userRole));
				}

				var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

				var token = new JwtSecurityToken(
							issuer: _configuration["JWT:ValidIssuer"],
							audience: _configuration["JWT:ValidAudience"],
							expires: DateTime.Now.AddHours(3),
							claims: authClaims,
							signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

				return new
				{
					token = new JwtSecurityTokenHandler().WriteToken(token),
					expiration = token.ValidTo
				};
			}
			return "User does not exists or invalid password input";
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

	}
}
