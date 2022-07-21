using System;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WA.Pizza.Infrastructure.Data;
using WA.Pizza.Infrastructure.Services.Mapster;
using Xunit;

namespace Pizzeria.Tests.Fixtures
{
    //[CollectionDefinition("Database collection")]
    //public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    //{

    //}

    public class DatabaseFixture : IDisposable
    {
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
            DbContext.Database.EnsureDeleted();
        }

        public AppDbContext DbContext { get; private set; }

    }
}
