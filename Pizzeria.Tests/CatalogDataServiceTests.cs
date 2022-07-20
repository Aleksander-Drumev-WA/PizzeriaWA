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
	public class CatalogDataServiceTests
	{

		[Fact]
		public async Task Show_catalog_items_successfully()
		{
			// Arrange
			using (var fixture = new DatabaseFixture())
			{
				var dbContext = fixture.DbContext;
				var catalogItemsToPass = Helper.GenerateCatalogItems(4, 25);
				dbContext.CatalogItems.AddRange(catalogItemsToPass);
				await dbContext.SaveChangesAsync();
				var sut = new CatalogDataService(dbContext);

				// Act
				var catalogItems = await sut.GetAllAsync();

				// Assert
				catalogItems.Should().BeEquivalentTo(dbContext.CatalogItems, options => options.ExcludingMissingMembers());

			}
		}
	}
}