using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.Data;
using WA.Pizza.Infrastructure.Data.Services;
using WA.Pizza.Infrastructure.DTO.Catalog;
using WA.Pizza.Infrastructure.Services.Mapster;
using WA.Pizza.Web.BackgroundJobs;
using WA.Pizza.Web.Extensions;
using WA.Pizza.Web.Filters;
using WA.Pizza.Web.Services.Validators;



var builder = WebApplication.CreateBuilder(args);

var seqConfig = "Serilog:Seq:Url";

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Information()
	.WriteTo.Console()
	.WriteTo.Seq(builder.Configuration.GetSection(seqConfig).Value)
	.CreateBootstrapLogger();

builder.Host.UseSerilog();

Log.Information("Starting up...");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddFluentValidation();

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

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
	builder.Configuration.GetConnectionString("Default")
	))
	.AddIdentityService()
	.AddJwtAuthentication(tokenValidationParams)
	.AddSwaggerConfig();

builder.Services.AddSingleton(tokenValidationParams);
builder.Services.AddScoped<BasketDataService>();
builder.Services.AddScoped<CatalogDataService>();
builder.Services.AddScoped<OrderDataService>();
builder.Services.AddScoped<AuthenticationDataService>();
builder.Services.AddScoped<AdvertisementDataService>();
builder.Services.AddScoped<AdsClientDataService>();

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
