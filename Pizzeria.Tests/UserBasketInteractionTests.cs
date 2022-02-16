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

namespace Pizzeria.Tests
{
    [Collection("Database collection")]
    public class UserBasketInteractionTests
    {
        [Fact]
        public async Task Anonymous_user_adds_catalog_item_to_basket()
        {
            // Arrange
            var fixture = new DatabaseFixture();
            var sut = new BasketDataService(fixture.DbContext);
            var catalogItem = new CatalogItem
            {
                Name = Lorem.Sentence(5),
                Price = RandomNumber.Next(1, int.MaxValue),
                PictureBytes = Lorem.Sentence(5),
                StorageQuantity = 110
            };
            await fixture.DbContext.CatalogItems.AddAsync(catalogItem);
            await fixture.DbContext.SaveChangesAsync();
            var request = new CatalogItemToBasketItemRequest
            {
                BasketId = null,
                CatalogItemId = catalogItem.Id,
                Quantity = 60
            };

            // Act
            var result = await sut.AddItemToBasketAsync(null, request);

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public async Task Registered_user_adds_catalog_item_to_basket()
        {
            // Arrange
            var fixture = new DatabaseFixture();
            var sut = new BasketDataService(fixture.DbContext);
            var user = new User
            {
                UserName = Internet.UserName(),
                Email = Internet.Email(),
                PasswordHash = Lorem.Sentence(8),
            };
            var catalogItem = new CatalogItem
            {
                Name = Lorem.Sentence(5),
                Price = RandomNumber.Next(1, int.MaxValue),
                PictureBytes = Lorem.Sentence(5),
                StorageQuantity = 110
            };
            await fixture.DbContext.CatalogItems.AddAsync(catalogItem);
            await fixture.DbContext.Users.AddAsync(user);
            await fixture.DbContext.SaveChangesAsync();
            var request = new CatalogItemToBasketItemRequest
            {
                BasketId = null,
                CatalogItemId = catalogItem.Id,
                Quantity = 60
            };

            // Act
            var result = await sut.AddItemToBasketAsync(user.Id, request);

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public async Task User_puts_more_quantity_than_storage_quantity()
        {
            // Arrange
            var fixture = new DatabaseFixture();
            var sut = new BasketDataService(fixture.DbContext);
            var catalogItem = new CatalogItem
            {
                Name = Lorem.Sentence(5),
                Price = RandomNumber.Next(1, int.MaxValue),
                PictureBytes = Lorem.Sentence(5),
                StorageQuantity = 110
            };
            await fixture.DbContext.CatalogItems.AddAsync(catalogItem);
            await fixture.DbContext.SaveChangesAsync();
            var request = new CatalogItemToBasketItemRequest
            {
                BasketId = null,
                CatalogItemId = catalogItem.Id,
                Quantity = 120
            };


            //sut.Invoking(async s => await s.AddItemToBasketAsync(null, request))
            //    .Should()
            //    .Throw<ArgumentException>()
            //    .WithInnerException<ArgumentException>()
            //    .WithMessage("Not enough stock in storage.");


            // Act, Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await sut.AddItemToBasketAsync(null, request));
        }

        [Fact]
        public async Task User_checks_his_basket_with_basket_items()
        {
            // Arrange
            var fixture = new DatabaseFixture();
            var sut = new BasketDataService(fixture.DbContext);
            var user = new User
            {
                UserName = Internet.UserName(),
                Email = Internet.Email(),
                PasswordHash = Lorem.Sentence(8),
            };
            await fixture.DbContext.Users.AddAsync(user);
            var catalogItems = new List<CatalogItem>
            {
                new CatalogItem
                 {
                     Name = Lorem.Sentence(5),
                     Price = RandomNumber.Next(1, int.MaxValue),
                     PictureBytes = Lorem.Sentence(5),
                     StorageQuantity = 25
                 },
                 new CatalogItem
                 {
                     Name = Lorem.Sentence(5),
                     Price = RandomNumber.Next(1, int.MaxValue),
                     PictureBytes = Lorem.Sentence(5),
                     StorageQuantity = 25
                 },
                 new CatalogItem
                 {
                     Name = Lorem.Sentence(5),
                     Price = RandomNumber.Next(1, int.MaxValue),
                     PictureBytes = Lorem.Sentence(5),
                     StorageQuantity = 25
                 },
                 new CatalogItem
                 {
                     Name = Lorem.Sentence(5),
                     Price = RandomNumber.Next(1, int.MaxValue),
                     PictureBytes = Lorem.Sentence(5),
                     StorageQuantity = 25
                 }
            };
            await fixture.DbContext.CatalogItems.AddRangeAsync(catalogItems);
            await fixture.DbContext.SaveChangesAsync();

            var basket = new Basket
            {
                UserId = user.Id
            };
            await fixture.DbContext.Baskets.AddAsync(basket);
            await fixture.DbContext.SaveChangesAsync();

            foreach (var catalogItem in catalogItems)
            {
                var request = new CatalogItemToBasketItemRequest
                {
                    BasketId = basket.Id,
                    CatalogItemId = catalogItem.Id,
                    Quantity = 5
                };

                await sut.AddItemToBasketAsync(user.Id, request);
            }

            // Act
            var result = sut.GetBasketWithBasketItems(basket.Id);

            // Assert
            result.Select(b => b.BasketItems.Should().HaveCount(4));
        }

        [Fact]
        public async Task User_changes_basketItem_quantity()
        {
            // Arrange
            var fixture = new DatabaseFixture();
            var sut = new BasketDataService(fixture.DbContext);
            var user = new User
            {
                UserName = Internet.UserName(),
                Email = Internet.Email(),
                PasswordHash = Lorem.Sentence(8),
            };
            await fixture.DbContext.Users.AddAsync(user);
            var catalogItems = new List<CatalogItem>
            {
                new CatalogItem
                 {
                     Name = Lorem.Sentence(5),
                     Price = RandomNumber.Next(1, int.MaxValue),
                     PictureBytes = Lorem.Sentence(5),
                     StorageQuantity = 25
                 },
                 new CatalogItem
                 {
                     Name = Lorem.Sentence(5),
                     Price = RandomNumber.Next(1, int.MaxValue),
                     PictureBytes = Lorem.Sentence(5),
                     StorageQuantity = 25
                 },
                 new CatalogItem
                 {
                     Name = Lorem.Sentence(5),
                     Price = RandomNumber.Next(1, int.MaxValue),
                     PictureBytes = Lorem.Sentence(5),
                     StorageQuantity = 25
                 },
                 new CatalogItem
                 {
                     Name = Lorem.Sentence(5),
                     Price = RandomNumber.Next(1, int.MaxValue),
                     PictureBytes = Lorem.Sentence(5),
                     StorageQuantity = 25
                 }
            };
            await fixture.DbContext.CatalogItems.AddRangeAsync(catalogItems);
            await fixture.DbContext.SaveChangesAsync();

            var basket = new Basket
            {
                UserId = user.Id
            };
            await fixture.DbContext.Baskets.AddAsync(basket);
            await fixture.DbContext.SaveChangesAsync();

            foreach (var catalogItem in catalogItems)
            {
                var request = new CatalogItemToBasketItemRequest
                {
                    BasketId = basket.Id,
                    CatalogItemId = catalogItem.Id,
                    Quantity = 5
                };

                await sut.AddItemToBasketAsync(user.Id, request);
            }

            var dto = new BasketItemDTO
            {
                BasketId = basket.Id,
                CatalogItemId = 2,
                Quantity = 22,
            };

            // Act
            var result = await sut.UpdateItemAsync(dto);

            // Assert
            result.Should().Be(22);
        }

        [Fact]
        public async Task User_removes_item_from_his_basket()
        {
            // Arrange
            var fixture = new DatabaseFixture();
            var sut = new BasketDataService(fixture.DbContext);
            var user = new User
            {
                UserName = Internet.UserName(),
                Email = Internet.Email(),
                PasswordHash = Lorem.Sentence(8),
            };
            await fixture.DbContext.Users.AddAsync(user);
            var catalogItems = new List<CatalogItem>
            {
                new CatalogItem
                 {
                     Name = Lorem.Sentence(5),
                     Price = RandomNumber.Next(1, int.MaxValue),
                     PictureBytes = Lorem.Sentence(5),
                     StorageQuantity = 25
                 },
                 new CatalogItem
                 {
                     Name = Lorem.Sentence(5),
                     Price = RandomNumber.Next(1, int.MaxValue),
                     PictureBytes = Lorem.Sentence(5),
                     StorageQuantity = 25
                 },
                 new CatalogItem
                 {
                     Name = Lorem.Sentence(5),
                     Price = RandomNumber.Next(1, int.MaxValue),
                     PictureBytes = Lorem.Sentence(5),
                     StorageQuantity = 25
                 },
                 new CatalogItem
                 {
                     Name = Lorem.Sentence(5),
                     Price = RandomNumber.Next(1, int.MaxValue),
                     PictureBytes = Lorem.Sentence(5),
                     StorageQuantity = 25
                 }
            };
            await fixture.DbContext.CatalogItems.AddRangeAsync(catalogItems);
            await fixture.DbContext.SaveChangesAsync();

            var basket = new Basket
            {
                UserId = user.Id
            };
            await fixture.DbContext.Baskets.AddAsync(basket);
            await fixture.DbContext.SaveChangesAsync();

            foreach (var catalogItem in catalogItems)
            {
                var request = new CatalogItemToBasketItemRequest
                {
                    BasketId = basket.Id,
                    CatalogItemId = catalogItem.Id,
                    Quantity = 5
                };

                await sut.AddItemToBasketAsync(user.Id, request);
            }

            // Act
            await sut.RemoveBasketItemAsync(3);

            // Assert
            basket.BasketItems.Should().HaveCount(3).And.NotContain(x => x.Id == 3);
        }
    }
}
