using WA.Pizza.Infrastructure.Data;
using WA.Pizza.Infrastructure.Data.Seed;
using WA.Pizza.Web.BackgroundJobs;

namespace WA.Pizza.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void SeedDatabase(this IApplicationBuilder applicationBuilder)
        {
            using var services = applicationBuilder.ApplicationServices.CreateScope();

            var dbContext = services.ServiceProvider.GetService<AppDbContext>();
            new CatalogItemSeeder().SeedAsync(dbContext).GetAwaiter().GetResult();
        }

        public static void CustomExceptionHandlerMiddleware(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}
