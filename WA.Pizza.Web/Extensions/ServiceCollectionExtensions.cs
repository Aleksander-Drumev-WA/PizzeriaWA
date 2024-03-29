﻿using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.BasketHandlers;
using WA.Pizza.Infrastructure.Data;
using WA.Pizza.Infrastructure.Data.Services;

namespace WA.Pizza.Web.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddIdentityService(this IServiceCollection services)
		{
			services.AddIdentity<User, Role>()
				.AddEntityFrameworkStores<AppDbContext>()
				.AddDefaultTokenProviders();

			services.Configure<IdentityOptions>(options =>
			{
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 6;
				options.Password.RequiredUniqueChars = 0;
			});

			return services;
		}

		public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, TokenValidationParameters tokenValidationParams)
		{
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.SaveToken = true;
				#if DEBUG
				options.RequireHttpsMetadata = false;
				#endif
				options.TokenValidationParameters = tokenValidationParams;
			});

			return services;
		}

		public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
		{
			services.AddSwaggerGen(swagger =>
			{
				swagger.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "ASP.NET Core 6 Web API"
				});

				swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
				{
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = $"Enter ‘Bearer’ [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\\"
				});

				swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						new string[] { }
					}
				});
			});

			return services;
		}

		public static IServiceCollection ConfigureCors(this IServiceCollection services)
		{
			services.AddCors(options =>
			{
				options.AddDefaultPolicy(policy =>
				{
					policy.AllowAnyHeader()
						  .AllowAnyMethod()
						  .AllowAnyOrigin();
				});
			});

			return services;
		}

		public static IServiceCollection InjectServices(this IServiceCollection services)
		{
			services.AddScoped<CatalogDataService>();
			services.AddScoped<OrderDataService>();
			services.AddScoped<AuthenticationDataService>();
			services.AddScoped<AdvertisementDataService>();
			services.AddScoped<AdsClientDataService>();
			services.AddMediatR(typeof(GetBasketQuery));
			services.AddSignalR();

			return services;
		}

		public static IServiceCollection InjectHangfire(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddHangfire(conf => conf
					.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
					.UseSimpleAssemblyNameTypeSerializer()
					.UseRecommendedSerializerSettings()
					.UseSqlServerStorage(configuration.GetConnectionString("Default")));

			services.AddHangfireServer();

			return services;
		}
	}
}
