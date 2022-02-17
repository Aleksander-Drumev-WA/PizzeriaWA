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
using System.Linq;
using WA.Pizza.Infrastructure.DTO.Basket;
using Pizzeria.Tests.Helpers;

namespace Pizzeria.Tests
{
    [Collection("Database collection")]
    public class BasketDataServiceTests
    {
        private readonly DatabaseFixture _fixture;

        public BasketDataServiceTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Anonymous_user_adds_catalog_item_to_basket()
        {
            // Arrange
            var catalogItems = DataGenerator.GenerateCatalogItems(1, 110);
            await _fixture.DbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _fixture.DbContext.SaveChangesAsync();
            var request = new CatalogItemToBasketItemRequest
            {
                BasketId = null,
                CatalogItemId = catalogItems[0].Id,
                Quantity = 60
            };
            var sut = new BasketDataService(_fixture.DbContext);

            // Act
            var basketId = await sut.AddItemToBasketAsync(null, request);

            // Assert
            var basketToAssert = await _fixture.DbContext.Baskets.FindAsync(basketId);
            basketToAssert.BasketItems
                .Should()
                .HaveCount(1)
                .And
                .Contain(x => x.Quantity == request.Quantity);

        }

        [Fact]
        public async Task Registered_user_adds_catalog_item_to_basket()
        {
            // Arrange
            var user = new User
            {
                UserName = Internet.UserName(),
                Email = Internet.Email(),
                PasswordHash = Lorem.Sentence(8),
            };
            var catalogItems = DataGenerator.GenerateCatalogItems(1, 110);
            await _fixture.DbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _fixture.DbContext.Users.AddAsync(user);
            await _fixture.DbContext.SaveChangesAsync();
            var request = new CatalogItemToBasketItemRequest
            {
                BasketId = null,
                CatalogItemId = catalogItems[0].Id,
                Quantity = 60
            };
            var sut = new BasketDataService(_fixture.DbContext);

            // Act
            var basketId = await sut.AddItemToBasketAsync(user.Id, request);

            // Assert
            var basketToAssert = await _fixture.DbContext.Baskets.FindAsync(basketId);
            basketToAssert.BasketItems
                .Should()
                .HaveCount(1)
                .And
                .Contain(x => x.Quantity == request.Quantity);
        }

        [Fact]
        public async Task User_puts_more_quantity_than_storage_quantity()
        {
            // Arrange
            var sut = new BasketDataService(_fixture.DbContext);
            var catalogItems = DataGenerator.GenerateCatalogItems(1, 110);
            await _fixture.DbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _fixture.DbContext.SaveChangesAsync();
            var request = new CatalogItemToBasketItemRequest
            {
                BasketId = null,
                CatalogItemId = catalogItems[0].Id,
                Quantity = 120
            };

            // Act
            Func<Task> act = () => sut.AddItemToBasketAsync(null, request);

            // Act, Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Not enough stock in storage.");
        }

        [Fact]
        public async Task User_checks_his_basket_with_basket_items()
        {
            // Arrange
            var user = new User
            {
                UserName = Internet.UserName(),
                Email = Internet.Email(),
                PasswordHash = Lorem.Sentence(8),
            };
            await _fixture.DbContext.Users.AddAsync(user);
            var catalogItems = DataGenerator.GenerateCatalogItems(4, 25);
            var basketItems = new List<BasketItem>();
            for (int i = 0; i < catalogItems.Count; i++)
            {
                basketItems.Add(new BasketItem
                {
                    CatalogItem = catalogItems[i],
                    Quantity = 5
                });
            }
            await _fixture.DbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _fixture.DbContext.SaveChangesAsync();

            var basket = new Basket
            {
                UserId = user.Id,
                BasketItems = basketItems
            };
            await _fixture.DbContext.Baskets.AddAsync(basket);
            await _fixture.DbContext.SaveChangesAsync();
            var sut = new BasketDataService(_fixture.DbContext);

            // Act
            var result = await sut.GetBasketWithBasketItemsAsync(basket.Id);

            // Assert
            var basketItemToAssert = await _fixture.DbContext.BasketItems.FindAsync(basketItems[2].Id);
            result.Select(b => b.BasketItems.Should().HaveCount(4));
            basketItemToAssert.Quantity.Should().Be(5);
        }

        [Fact]
        public async Task User_changes_basketItem_quantity()
        {
            // Arrange
            var user = new User
            {
                UserName = Internet.UserName(),
                Email = Internet.Email(),
                PasswordHash = Lorem.Sentence(8),
            };
            await _fixture.DbContext.Users.AddAsync(user);
            var catalogItems = DataGenerator.GenerateCatalogItems(4, 25);
            var basketItems = new List<BasketItem>();
            for (int i = 0; i < 4; i++)
            {
                basketItems.Add(new BasketItem
                {
                    CatalogItem = catalogItems[i],
                    Quantity = i + 1,
                });
            }
            await _fixture.DbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _fixture.DbContext.SaveChangesAsync();
            var basket = new Basket
            {
                UserId = user.Id,
                BasketItems = basketItems
            };
            await _fixture.DbContext.Baskets.AddAsync(basket);
            await _fixture.DbContext.SaveChangesAsync();
            var dto = new BasketItemDTO
            {
                BasketId = basket.Id,
                CatalogItemId = catalogItems[2].Id,
                Quantity = 22,
                Name = catalogItems[2].Name,
                PictureBytes = catalogItems[2].PictureBytes,
                Price = catalogItems[2].Price,
            };
            var sut = new BasketDataService(_fixture.DbContext);

            // Act
            var updatedItemId = await sut.UpdateItemAsync(dto);

            // Assert
            var updatedCatalogItem = await _fixture.DbContext.BasketItems.FindAsync(updatedItemId);
            updatedCatalogItem.Quantity.Should().Be(22);
        }

        [Fact]
        public async Task User_removes_item_from_his_basket()
        {
            // Arrange
            var user = new User
            {
                UserName = Internet.UserName(),
                Email = Internet.Email(),
                PasswordHash = Lorem.Sentence(8),
            };
            await _fixture.DbContext.Users.AddAsync(user);
            var catalogItems = DataGenerator.GenerateCatalogItems(4, 25);
            var basketItems = new List<BasketItem>();
            for (int i = 0; i < 4; i++)
            {
                basketItems.Add(new BasketItem
                {
                    CatalogItem = catalogItems[i],
                    Quantity = i + 1,
                });
            }
            await _fixture.DbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _fixture.DbContext.SaveChangesAsync();

            var basket = new Basket
            {
                UserId = user.Id,
                BasketItems = basketItems,
            };
            await _fixture.DbContext.Baskets.AddAsync(basket);
            await _fixture.DbContext.SaveChangesAsync();
            var sut = new BasketDataService(_fixture.DbContext);

            // Act
            await sut.RemoveBasketItemAsync(basketItems[3].Id);

            // Assert
            basket.BasketItems.Should().HaveCount(3).And.NotContain(x => x.Id == 4);
        }
    }
}
