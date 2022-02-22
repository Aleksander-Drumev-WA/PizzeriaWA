using WA.Pizza.Infrastructure.Data;
using WA.Pizza.Infrastructure.Data.Seed;

namespace WA.Pizza.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void SeedDatabase(this IApplicationBuilder applicationBuilder)
        {
            using var services = applicationBuilder.ApplicationServices.CreateScope();

            var dbContext = services.ServiceProvider.GetService<AppDbContext>();
            new DbContextSeeder().SeedAsync(dbContext).GetAwaiter().GetResult();
        }
    }
}
