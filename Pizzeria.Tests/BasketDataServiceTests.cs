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
        private readonly AppDbContext _dbContext;

        public BasketDataServiceTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
        }

        [Fact]
        public async Task Anonymous_user_adds_catalog_item_to_basket()
        {
            // Arrange
            var catalogItems = Helper.GenerateCatalogItems(1, 110);
            await _dbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _dbContext.SaveChangesAsync();
            var catalogItem = catalogItems.First();
            var request = new CatalogItemToBasketItemRequest
            {
                BasketId = null,
                CatalogItemId = catalogItem.Id,
                Quantity = 60,
                Name = catalogItem.Name,
                Price = catalogItem.Price,
            };
            var sut = new BasketDataService(_dbContext);

            // Act
            var basketId = await sut.AddItemToBasketAsync(request);

            // Assert
            var basketToAssert = await _dbContext.Baskets.FindAsync(basketId);
            basketToAssert.Should().NotBeNull();
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
            var catalogItems = Helper.GenerateCatalogItems(1, 110);
            await _dbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            var catalogItem = catalogItems.First();
            var request = new CatalogItemToBasketItemRequest
            {
                BasketId = null,
                CatalogItemId = catalogItem.Id,
                Quantity = 60,
                Name = catalogItem.Name,
                Price = catalogItem.Price,
            };
            var sut = new BasketDataService(_dbContext);

            // Act
            var basketId = await sut.AddItemToBasketAsync(request, user.Id);

            // Assert
            var basketToAssert = await _dbContext.Baskets.FindAsync(basketId);
            basketToAssert.Should().NotBeNull();
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
            var sut = new BasketDataService(_dbContext);
            var catalogItems = Helper.GenerateCatalogItems(1, 110);
            await _dbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _dbContext.SaveChangesAsync();
            var request = new CatalogItemToBasketItemRequest
            {
                BasketId = null,
                CatalogItemId = catalogItems[0].Id,
                Quantity = 120
            };

            // Act
            Func<Task> act = () => sut.AddItemToBasketAsync(request);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Not enough stock in storage.");
        }

        [Fact]
        public async Task User_gets_his_basket_with_basket_items()
        {
            // Arrange
            var user = new User
            {
                UserName = Internet.UserName(),
                Email = Internet.Email(),
                PasswordHash = Lorem.Sentence(8),
            };
            await _dbContext.Users.AddAsync(user);
            var catalogItems = Helper.GenerateCatalogItems(4, 25);
            await _dbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _dbContext.SaveChangesAsync();
            var basketItems = new List<BasketItem>();
            for (int i = 0; i < catalogItems.Count; i++)
            {
                basketItems.Add(new BasketItem
                {
                    CatalogItemId = catalogItems[i].Id,
                    Quantity = 5,
                    Name = catalogItems[i].Name,
                    Price = catalogItems[i].Price,
                });
            }
            var basket = new Basket
            {
                UserId = user.Id,
                BasketItems = basketItems
            };
            await _dbContext.Baskets.AddAsync(basket);
            await _dbContext.SaveChangesAsync();
            var sut = new BasketDataService(_dbContext);

            // Act
            var result = await sut.GetBasketWithBasketItemsAsync(basket.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Select(b => b.BasketItems.Should().HaveCount(4));
            result.Select(b => b.BasketItems.All(bi => bi.Quantity == 5).Should().BeTrue());
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
            await _dbContext.Users.AddAsync(user);
            var catalogItems = Helper.GenerateCatalogItems(4, 25);
            await _dbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _dbContext.SaveChangesAsync();
            var basketItems = new List<BasketItem>();

            for (int i = 0; i < catalogItems.Count; i++)
            {
                basketItems.Add(new BasketItem
                {
                    CatalogItemId = catalogItems[i].Id,
                    Quantity = i + 1,
                    Name = catalogItems[i].Name,
                    Price = catalogItems[i].Price,
                });
            }
            var basket = new Basket
            {
                UserId = user.Id,
                BasketItems = basketItems
            };
            await _dbContext.Baskets.AddAsync(basket);
            await _dbContext.SaveChangesAsync();
            var dto = new BasketItemDTO
            {
                BasketId = basket.Id,
                CatalogItemId = catalogItems[0].Id,
                Quantity = 22,
                Name = catalogItems[0].Name,
                Price = catalogItems[0].Price,
            };
            var sut = new BasketDataService(_dbContext);

            // Act
            var updatedItemId = await sut.UpdateItemAsync(dto);

            // Assert
            var updatedCatalogItem = await _dbContext.BasketItems.FindAsync(updatedItemId);
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
            await _dbContext.Users.AddAsync(user);
            var catalogItems = Helper.GenerateCatalogItems(4, 25);
            await _dbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _dbContext.SaveChangesAsync();
            var basketItems = new List<BasketItem>();
            for (int i = 0; i < catalogItems.Count; i++)
            {
                basketItems.Add(new BasketItem
                {
                    CatalogItemId = catalogItems[i].Id,
                    Quantity = i + 1,
                    Name = catalogItems[i].Name,
                    Price = catalogItems[i].Price,
                });
            }
            var basket = new Basket
            {
                UserId = user.Id,
                BasketItems = basketItems,
            };
            var basketItemToTest = basketItems.First();
            await _dbContext.Baskets.AddAsync(basket);
            await _dbContext.SaveChangesAsync();
            var sut = new BasketDataService(_dbContext);

            // Act
            await sut.RemoveBasketItemAsync(basketItemToTest.Id);

            // Assert
            var ensureDeletedBasketItem = await _dbContext.BasketItems.FindAsync(basketItemToTest.Id);
            ensureDeletedBasketItem.Should().BeNull();
            basket.BasketItems.Should().HaveCount(3).And.NotContain(x => x.Id == basketItemToTest.Id);
        }
    }
}
