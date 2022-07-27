using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WA.Pizza.Infrastructure.Data;
using WA.Pizza.Infrastructure.Services.Mapster;
using Xunit;

namespace Pizzeria.Tests.Fixtures
{
	public class InMemoryDatabaseFixture : IDisposable
	{
		private readonly AppDbContext _dbContext;

		public AppDbContext DbContext => _dbContext;

		public InMemoryDatabaseFixture()
		{
			var builder = new DbContextOptionsBuilder<AppDbContext>();
			builder.UseInMemoryDatabase(databaseName: "LibraryDbInMemory");

			var dbContextOptions = builder.Options;
			_dbContext = new AppDbContext(dbContextOptions);
			MappingConfig.Configure();
		}

		public void Dispose()
		{
			DbContext.Database.EnsureDeleted();
		}
	}
}
