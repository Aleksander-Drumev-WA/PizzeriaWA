using Hangfire;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WA.Pizza.Infrastructure.Data;

namespace WA.Pizza.Web.BackgroundJobs
{
	public class ForgottenBasketsJob 
	{
		private readonly AppDbContext _dbContext;
		private readonly ILogger<ForgottenBasketsJob> _logger;

		public ForgottenBasketsJob(AppDbContext dbContext, ILogger<ForgottenBasketsJob> logger)
		{
			_dbContext = dbContext;
			_logger = logger;
		}

		public async Task RunAsync()
		{
			var usersWhoForgotBasket = await _dbContext
				.Baskets
				.Include(b => b.BasketItems)
				.Include(b => b.User)
				.Where(b => b.BasketItems.Any() &&
							b.LastModifiedOn.HasValue &&
							DateTime.UtcNow.Subtract(b.LastModifiedOn.Value) >= TimeSpan.FromDays(7))
				.Select(b => b.User)
				.ToListAsync();

			_logger.LogInformation($"Users: { string.Join(", ", usersWhoForgotBasket.Select(u => u.UserName ?? "Anonymous user")) } have forgottern their basket.");

			foreach (var user in usersWhoForgotBasket)
			{
				user.Basket.LastModifiedOn = null;
			}

			await _dbContext.SaveChangesAsync();
		}
	}
}
