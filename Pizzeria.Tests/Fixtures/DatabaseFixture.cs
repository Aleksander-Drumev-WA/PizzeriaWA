using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Respawn.Graph;
using WA.Pizza.Infrastructure.Data;
using WA.Pizza.Infrastructure.Services.Mapster;
using Xunit;

namespace Pizzeria.Tests.Fixtures
{
	[CollectionDefinition("Database collection", DisableParallelization = true)]
	public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
	{

	}

	public class DatabaseFixture : IDisposable
	{

		private static Checkpoint checkpoint = new Checkpoint()
		{
			DbAdapter = DbAdapter.SqlServer,
			TablesToIgnore = new Table[]
			{
				"__EFMigrationsHistory"
			}
		};
		private DbContextOptions<AppDbContext> _options { get; set; }

		private SqlConnection _connection { get; set; }

		public DatabaseFixture()
		{
			_connection = new SqlConnection("Server=localhost,5121;Database=WA.PizzaDB.Tests;Trusted_Connection=False;MultipleActiveResultSets=true;User=sa;Password=#sql-pass22_;TrustServerCertificate=True");
			_options = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(_connection.ConnectionString).Options;
			DbContext = new AppDbContext(_options);
			DbContext.Database.Migrate();
			MappingConfig.Configure();
		}

		public void Dispose()
		{
			checkpoint.Reset(_connection.ConnectionString).GetAwaiter().GetResult();
		}

		public AppDbContext DbContext { get; private set; }

	}
}
