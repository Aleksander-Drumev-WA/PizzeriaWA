using Xunit;
using FluentAssertions;
using Faker;
using WA.Pizza.Infrastructure.Data;
using WA.Pizza.Infrastructure.Data.Services;
using WA.Pizza.Infrastructure.DTO.Catalog;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;
using System;

using Pizzeria.Tests.Fixtures;
using System.Collections.Generic;
using Pizzeria.Tests.Helpers;
using System.Linq;

namespace Pizzeria.Tests
{
    [Collection("In-Memory Database Collection")]
    public class CatalogDataServiceTests
    {
        private readonly AppDbContext _dbContext;

        public CatalogDataServiceTests(InMemoryDatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;

        }

        [Fact]
        public async Task Show_catalog_items_successfully()
        {
            // Arrange
            var catalogItemsToPass = Helper.GenerateCatalogItems(4, 25);
            await _dbContext.CatalogItems.AddRangeAsync(catalogItemsToPass);
            await _dbContext.SaveChangesAsync();
            var sut = new CatalogDataService(_dbContext);

            // Act
            var catalogItems = await sut.GetAllAsync();

            // Assert
            var catalogItemToAssert = await _dbContext.CatalogItems.FindAsync(catalogItems.First().Id);
            var comparisonItem = catalogItemsToPass.First();
            catalogItems
                .Should()
                .NotBeEmpty()
                .And
                .HaveCount(4);
            catalogItemToAssert.Should().BeEquivalentTo(comparisonItem);
        }
    }
}