using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using WA.Pizza.Infrastructure.Data;
using WA.Pizza.Infrastructure.Services.Mapster;
using WA.Pizza.Web.BackgroundJobs;
using WA.Pizza.Web.Extensions;
using WA.Pizza.Web.Filters;
using WA.Pizza.Web.Hubs;

try
{
	var builder = WebApplication.CreateBuilder(args);

	var seqConfig = "Serilog:Seq:Url";

	var configuration = builder.Configuration;

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
		ValidAudience = configuration["JWT:ValidAudience"],
		ValidIssuer = configuration["JWT:ValidIssuer"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
	};

	builder.Services.AddSingleton(tokenValidationParams);

	builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
		builder.Configuration.GetConnectionString("Default")
		))
		.AddIdentityService()
		.InjectServices()
		.AddJwtAuthentication(tokenValidationParams)
		.AddSwaggerConfig()
		.ConfigureCors()
		.InjectHangfire(configuration);

	MappingConfig.Configure();

	var app = builder.Build();

	// Configure the HTTP request pipeline.
	if (!app.Environment.IsDevelopment())
	{
		app.UseExceptionHandler("/Home/Error");
		// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
		app.UseHsts();
	}
	app.UseExceptionHandler(ab => ab.ExceptionHandlerConfigure(app.Environment));
	app.SeedDatabase();
	app.UseSerilogRequestLogging();
	app.UseHttpsRedirection();


	app.UseRouting();
	app.UseStaticFiles();
	app.UseCors();

	app.UseSwagger();
	app.UseSwaggerUI(s =>
	{
		s.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
	});
	app.UseHangfireDashboard("/hangfire", new DashboardOptions
	{
		Authorization = new[] { new HangfireAuthorizationFilter() }
	});

	app.UseAuthentication();
	app.UseAuthorization();

	RecurringJob.AddOrUpdate<ForgottenBasketsJob>("forgottenBasketsJob", job => job.RunAsync(), Cron.Weekly);

	app.UseEndpoints(endpoints =>
	{
		endpoints.MapControllers();
		endpoints.MapHub<ChatHub>("/hubs/chat");
	});

	await app.RunAsync();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application accidentally crashed!");
	throw;
}
finally
{
	Log.CloseAndFlush();
}