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

namespace Pizzeria.Tests
{
    [Collection("Database collection")]
    public class CatalogDataServiceTests
    {
        private readonly DatabaseFixture _fixture;

        public CatalogDataServiceTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Show_catalog_items_successfully()
        {
            // Arrange
            var catalogItemsToPass = DataGenerator.GenerateCatalogItems(4, 25);
            await _fixture.DbContext.CatalogItems.AddRangeAsync(catalogItemsToPass);
            await _fixture.DbContext.SaveChangesAsync();
            var sut = new CatalogDataService(_fixture.DbContext);

            // Act
            var catalogItems = await sut.GetAllAsync();

            // Assert
            var catalogItemToAssert = await _fixture.DbContext.CatalogItems.FindAsync(catalogItems[1].Id);
            var comparisonItem = catalogItemsToPass[1];
            catalogItems
                .Should()
                .NotBeEmpty()
                .And
                .HaveCount(4);
            catalogItemToAssert.Should().NotBeNull();
            catalogItemToAssert.Name.Should().Be(comparisonItem.Name);
            catalogItemToAssert.Price.Should().Be(comparisonItem.Price);
            catalogItemToAssert.PictureBytes.Should().Be(comparisonItem.PictureBytes);
            catalogItemToAssert.StorageQuantity.Should().Be(comparisonItem.StorageQuantity);
        }
    }
}