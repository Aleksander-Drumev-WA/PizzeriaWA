using Hangfire;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WA.Pizza.Infrastructure.Data;

namespace WA.Pizza.Web.BackgroundJobs
{
	public class ForgottenBasketsJob 
	{
		private readonly AppDbContext _dbContext;

		public ForgottenBasketsJob(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task NotifyUsersAboutForgottenBasket()
		{
			var usersWhoForgotBasket = await _dbContext
				.Baskets
				.Include(b => b.BasketItems)
				.Where(b => b.BasketItems.Any())
				.Select(b => b.UserId.ToString() ?? "Anonymous user")
				.ToListAsync();

			RecurringJob.AddOrUpdate("test12", () => Log.Information($"Users: {string.Join(", ", usersWhoForgotBasket)} have forgotten their basket."), Cron.Monthly);
		}
	}
}
