using Pizzeria.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;
using Xunit;
using Faker;
using FluentAssertions;
using Pizzeria.Tests.Helpers;
using WA.Pizza.Infrastructure.Data.Services;
using WA.Pizza.Infrastructure.Data;

namespace Pizzeria.Tests
{
    [Collection("Database collection")]
    public class OrderDataServiceTests
    {
        private readonly AppDbContext _dbContext;

        public OrderDataServiceTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
        }

        [Fact]
        public async Task User_creates_order()
        {
            // Arrange
            var user = new User
            {
                UserName = Internet.UserName("test"),
                Email = Internet.Email("test"),
                PasswordHash = Lorem.Sentence(8)
            };
            await _dbContext.Users.AddAsync(user);
            var catalogItems = Helper.GenerateCatalogItems(4, 40);
            await _dbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _dbContext.SaveChangesAsync();
            var basketItems = new List<BasketItem>();
            for (int i = 0; i < catalogItems.Count; i++)
            {
                basketItems.Add(new BasketItem
                {
                    CatalogItemId = catalogItems[i].Id,
                    Quantity = i + 2,
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
            var sut = new OrderDataService(_dbContext);

            // Act
            var orderId = await sut.CreateOrderAsync(basket.Id);

            // Assert
            var orderToAssert = await _dbContext.Orders.FindAsync(orderId);
            orderToAssert.OrderItems.Should().NotBeNull();
            orderToAssert.OrderItems.Should().HaveCount(catalogItems.Count);
        }

        [Fact]
        public async Task User_gets_all_his_orders()
        {
            // Arrange
            var user = new User
            {
                UserName = Internet.UserName("test"),
                Email = Internet.Email("test"),
                PasswordHash = Lorem.Sentence(8)
            };
            await _dbContext.Users.AddAsync(user);
            var catalogItems = Helper.GenerateCatalogItems(4, 40);
            await _dbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _dbContext.SaveChangesAsync();
            var basketItems = new List<BasketItem>();
            for (int i = 0; i < catalogItems.Count; i++)
            {
                basketItems.Add(new BasketItem
                {
                    CatalogItemId = catalogItems[i].Id,
                    Quantity = i + 2,
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
            var sut = new OrderDataService(_dbContext);
            await sut.CreateOrderAsync(basket.Id);
            await sut.CreateOrderAsync(basket.Id);
            await sut.CreateOrderAsync(basket.Id);

            // Act
            var resultList = await sut.GetMyOrdersAsync(user.Id);

            // Assert
            resultList.Should().NotBeNull();
            resultList.Should().HaveCount(3);
            resultList.All(list => list.OrderItems != null).Should().BeTrue();
        }

        [Fact]
        public async Task User_changes_order_status()
        {
            // Arrange
            var user = new User
            {
                UserName = Internet.UserName("test"),
                Email = Internet.Email("test"),
                PasswordHash = Lorem.Sentence(8)
            };
            await _dbContext.Users.AddAsync(user);
            var catalogItems = Helper.GenerateCatalogItems(4, 40);
            await _dbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _dbContext.SaveChangesAsync();
            var basketItems = new List<BasketItem>();
            for (int i = 0; i < catalogItems.Count; i++)
            {
                basketItems.Add(new BasketItem
                {
                    CatalogItemId = catalogItems[i].Id,
                    Quantity = i + 2,
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
            var sut = new OrderDataService(_dbContext);
            var orderId = await sut.CreateOrderAsync(basket.Id);

            // Act
            await sut.UpdateOrderStatusAsync(orderId, OrderStatus.Completed);

            // Assert
            var updatedOrder = await _dbContext.Orders.FindAsync(orderId);
            updatedOrder.Should().NotBeNull();
            updatedOrder.OrderStatus.Should().Be(OrderStatus.Completed);
        }

        [Fact]
        public async Task User_gets_one_of_his_orders()
        {
            // Arrange
            var user = new User
            {
                UserName = Internet.UserName("test"),
                Email = Internet.Email("test"),
                PasswordHash = Lorem.Sentence(8)
            };
            await _dbContext.Users.AddAsync(user);
            var catalogItems = Helper.GenerateCatalogItems(4, 40);
            await _dbContext.CatalogItems.AddRangeAsync(catalogItems);
            await _dbContext.SaveChangesAsync();
            var basketItems = new List<BasketItem>();
            for (int i = 0; i < catalogItems.Count; i++)
            {
                basketItems.Add(new BasketItem
                {
                    CatalogItemId = catalogItems[i].Id,
                    Quantity = i + 2,
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
            var sut = new OrderDataService(_dbContext);
            var orderId = await sut.CreateOrderAsync(basket.Id);

            // Act
            var order = await sut.GetOrderAsync(orderId);

            // Assert
            order.Should().NotBeNull();
            order.OrderItems.All(oi => oi != null).Should().BeTrue();
            order.OrderItems.Should().HaveCount(basketItems.Count);
        }
    }
}
