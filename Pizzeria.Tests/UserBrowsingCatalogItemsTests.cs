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

namespace Pizzeria.Tests
{
    [Collection("Database collection")]
    public class UserBrowsingCatalogItemsTests
    {
        [Fact]
        public async Task Create_And_Get_All_Catalog_Items()
        {
            // Arrange
            var fixture = new DatabaseFixture();
            var sut = new CatalogDataService(fixture.DbContext);
            var dataToPass = new List<CatalogItem>
            {
                 new CatalogItem
                 {
                     Name = Lorem.Sentence(5),
                     Price = RandomNumber.Next(1, int.MaxValue),
                     PictureBytes = Lorem.Sentence(5),
                     StorageQuantity = RandomNumber.Next(1, 1500)
                 },
                 new CatalogItem
                 {
                     Name = Lorem.Sentence(5),
                     Price = RandomNumber.Next(1, int.MaxValue),
                     PictureBytes = Lorem.Sentence(5),
                     StorageQuantity = RandomNumber.Next(1, 1500)
                 },
                 new CatalogItem
                 {
                     Name = Lorem.Sentence(5),
                     Price = RandomNumber.Next(1, int.MaxValue),
                     PictureBytes = Lorem.Sentence(5),
                     StorageQuantity = RandomNumber.Next(1, 1500)
                 },
                 new CatalogItem
                 {
                     Name = Lorem.Sentence(5),
                     Price = RandomNumber.Next(1, int.MaxValue),
                     PictureBytes = Lorem.Sentence(5),
                     StorageQuantity = RandomNumber.Next(1, 1500)
                 }
            };
            await fixture.DbContext.CatalogItems.AddRangeAsync(dataToPass);
            await fixture.DbContext.SaveChangesAsync();

            // Act
            var catalogItems = sut.GetAll();

            // Assert
            catalogItems
                .Should()
                .NotBeEmpty()
                .And
                .HaveCount(4);
        }

        //[Fact]
        //public async Task Get_One_Catalog_Item()
        //{
        //    var catalogItem = _sut.GetOneCatalogItemAsync(2);
        //}
    }
}