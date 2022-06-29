using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.BasketHandlers;
using WA.Pizza.Infrastructure.Data;
using WA.Pizza.Infrastructure.Data.Services;
using WA.Pizza.Infrastructure.DTO.Catalog;
using WA.Pizza.Infrastructure.Services.Mapster;
using WA.Pizza.Web.BackgroundJobs;
using WA.Pizza.Web.Extensions;
using WA.Pizza.Web.Filters;
using WA.Pizza.Web.Services.Validators;



var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Information()
	.WriteTo.Console()
	.WriteTo.Seq(builder.Configuration.GetSection("Serilog").GetSection("Seq").GetSection("Url").Value)
	.CreateBootstrapLogger();

builder.Host.UseSerilog();

Log.Information("Starting up...");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddFluentValidation();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
	builder.Configuration.GetConnectionString("Default")
	));

builder.Services.AddIdentity<User, Role>()
				.AddEntityFrameworkStores<AppDbContext>()
				.AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
	options.Password.RequireDigit = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = false;
	options.Password.RequiredLength = 6;
	options.Password.RequiredUniqueChars = 0;
});

var tokenValidationParams = new TokenValidationParameters()
{
	ValidateIssuer = true,
	ValidateAudience = true,
	ValidateLifetime = true,
	ValidateIssuerSigningKey = true,
	ValidAudience = builder.Configuration["JWT:ValidAudience"],
	ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
	IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
};

builder.Services.AddAuthentication(options =>
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

builder.Services.AddSwaggerGen(swagger =>
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

builder.Services.AddSingleton(tokenValidationParams);
builder.Services.AddScoped<CatalogDataService>();
builder.Services.AddScoped<OrderDataService>();
builder.Services.AddScoped<AuthenticationDataService>();
builder.Services.AddScoped<AdvertisementDataService>();
builder.Services.AddScoped<AdsClientDataService>();
builder.Services.AddMediatR(typeof(GetBasketQuery));

MappingConfig.Configure();


builder.Services.AddHangfire(configuration => configuration
		.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
		.UseSimpleAssemblyNameTypeSerializer()
		.UseRecommendedSerializerSettings()
		.UseSqlServerStorage(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
app.UseExceptionHandler(ab => ab.ExceptionHandlerConfigure(app.Environment));
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(s =>
{
	s.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
	Authorization = new [] { new HangfireAuthorizationFilter() }
});
RecurringJob.AddOrUpdate<ForgottenBasketsJob>("forgottenBasketsJob", job => job.RunAsync(), Cron.Weekly);

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.SeedDatabase();
app.Run();

Log.CloseAndFlush();

Console.ReadKey(true);
