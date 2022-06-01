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
                UserId = null,
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
                .Contain(
                bi => bi.Quantity == request.Quantity &&
                bi.Name == request.Name && 
                bi.Price == request.Price && 
                bi.CatalogItemId == request.CatalogItemId &&
                bi.BasketId != null && 
                bi.Basket.UserId == null);
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
                UserId = user.Id,
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
            basketToAssert.UserId.Should().NotBeNull();
            basketToAssert.UserId.Should().Be(user.Id);
            basketToAssert.BasketItems
                .Should()
                .HaveCount(1)
                .And
                .Contain(
                bi => bi.Quantity == request.Quantity &&
                bi.Name == request.Name &&
                bi.Price == request.Price &&
                bi.CatalogItemId == request.CatalogItemId &&
                bi.BasketId != null);
        }

        [Fact]
        public async Task User_puts_more_quantity_than_storage_quantity()
        {
            // Arrange
            var sut = new BasketDataService(_dbContext);
            var catalogItems = Helper.GenerateCatalogItems(1, 110);
            await _dbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _dbContext.SaveChangesAsync();
            var catalogItem = catalogItems.First();
            var request = new CatalogItemToBasketItemRequest
            {
                BasketId = null,
                UserId = null,
                CatalogItemId = catalogItem.Id,
                Quantity = 120,
                Name = catalogItem.Name,
                Price = catalogItem.Price
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
            foreach (var catalogItem in catalogItems)
            {
                basketItems.Add(new BasketItem
                {
                    CatalogItemId = catalogItem.Id,
                    Quantity = 5,
                    Name = catalogItem.Name,
                    Price = catalogItem.Price,
                });
            }
            var basket = new Basket
            {
                UserId = user.Id,
                BasketItems = basketItems
            };
            var basketItem = basketItems.First();
            await _dbContext.Baskets.AddAsync(basket);
            await _dbContext.SaveChangesAsync();
            var sut = new BasketDataService(_dbContext);

            // Act
            var result = await sut.GetBasketWithBasketItemsAsync(basket.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Select(b => b.BasketItems.Should().HaveCount(4));
            result.Select(b => b.BasketItems.All(bi => bi.Quantity == basketItem.Quantity).Should().BeTrue());
        }

        [Fact]
        public async Task User_changes_basketItem_quantity_successfully()
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
            foreach (var currentCatalogItem in catalogItems)
            {
                basketItems.Add(new BasketItem
                {
                    CatalogItemId = currentCatalogItem.Id,
                    Quantity = 5,
                    Name = currentCatalogItem.Name,
                    Price = currentCatalogItem.Price,
                });
            }
            var basket = new Basket
            {
                UserId = user.Id,
                BasketItems = basketItems
            };
            await _dbContext.Baskets.AddAsync(basket);
            await _dbContext.SaveChangesAsync();
            var firstCatalogItem = catalogItems.First();
            var dto = new BasketItemDTO
            {
                Id = 1,
                BasketId = basket.Id,
                CatalogItemId = firstCatalogItem.Id,
                Quantity = 22,
                Name = firstCatalogItem.Name,
                Price = firstCatalogItem.Price,
            };
            var resultStorageQuantity = firstCatalogItem.StorageQuantity - dto.Quantity;
            var sut = new BasketDataService(_dbContext);

            // Act
            var updatedItemId = await sut.UpdateItemAsync(dto);

            // Assert
            var updatedCatalogItem = await _dbContext.BasketItems.FindAsync(updatedItemId);
            updatedCatalogItem!.Quantity.Should().Be(22);
            updatedCatalogItem.CatalogItem.StorageQuantity.Should().Be(resultStorageQuantity);
        }

        [Fact]
        public async Task User_tries_to_order_more_than_we_have_on_storage()
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
            foreach (var currentCatalogItem in catalogItems)
            {
                basketItems.Add(new BasketItem
                {
                    CatalogItemId = currentCatalogItem.Id,
                    Quantity = 5,
                    Name = currentCatalogItem.Name,
                    Price = currentCatalogItem.Price,
                });
            }
            var basket = new Basket
            {
                UserId = user.Id,
                BasketItems = basketItems
            };
            await _dbContext.Baskets.AddAsync(basket);
            await _dbContext.SaveChangesAsync();
            var firstCatalogItem = catalogItems.First();
            var dto = new BasketItemDTO
            {
                Id = 1,
                BasketId = basket.Id,
                CatalogItemId = firstCatalogItem.Id,
                Quantity = 2200,
                Name = firstCatalogItem.Name,
                Price = firstCatalogItem.Price,
            };
            var sut = new BasketDataService(_dbContext);

            // Act
            Func<Task> act = () => sut.UpdateItemAsync(dto);

            //Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Not enough stock in storage.");
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
            foreach (var catalogItem in catalogItems)
            {
                basketItems.Add(new BasketItem
                {
                    CatalogItemId = catalogItem.Id,
                    Quantity = 5,
                    Name = catalogItem.Name,
                    Price = catalogItem.Price,
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
            await sut.RemoveBasketItem(basketItemToTest.Id);

            // Assert
            var ensureDeletedBasketItem = await _dbContext.BasketItems.FindAsync(basketItemToTest.Id);
            ensureDeletedBasketItem.Should().BeNull();
            basket.BasketItems.Should().HaveCount(3).And.NotContain(x => x.Id == basketItemToTest.Id);
        }
    }
}
