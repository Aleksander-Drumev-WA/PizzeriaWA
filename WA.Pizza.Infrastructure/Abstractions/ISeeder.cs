using WA.Pizza.Infrastructure.Data;

namespace WA.Pizza.Infrastructure.Abstractions
{
    public interface ISeeder
    {
        public Task SeedAsync(AppDbContext dbContext);
    }
}
