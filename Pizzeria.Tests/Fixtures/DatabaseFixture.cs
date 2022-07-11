﻿using System;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WA.Pizza.Infrastructure.Data;
using WA.Pizza.Infrastructure.Services.Mapster;
using Xunit;

namespace Pizzeria.Tests.Fixtures
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {

    }

    public class DatabaseFixture : IDisposable
    {
        private DbContextOptions<AppDbContext> _options { get; set; }

        private string _connection { get; set; }

        public DatabaseFixture()
        {
            _connection = "Server=localhost,5121;Database=WA.PizzaDB.Tests;User=sa;Password=#sql-pass22_";
            _options = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(_connection).Options;
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
