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
using WA.Pizza.Core.Exceptions;

namespace Pizzeria.Tests
{
	public class OrderDataServiceTests
	{

		[Fact]
		public async Task User_creates_order()
		{
			// Arrange
			using (var fixture = new InMemoryDatabaseFixture())
			{
				var dbContext = fixture.DbContext;
				var user = new User
				{
					UserName = Internet.UserName("test"),
					Email = Internet.Email("test"),
					PasswordHash = Lorem.Sentence(8)
				};
				user.NormalizedEmail = user.Email.ToUpper();
				user.NormalizedUserName = user.UserName.ToUpper();
				dbContext.Users.Add(user);
				var catalogItems = Helper.GenerateCatalogItems(4, 40);
				dbContext.CatalogItems.AddRange(catalogItems);
				await dbContext.SaveChangesAsync();
				var basketItems = new List<BasketItem>();
				foreach (var catalogItem in catalogItems)
				{
					basketItems.Add(new BasketItem
					{
						CatalogItemId = catalogItem.Id,
						Quantity = 10,
						Name = catalogItem.Name,
						Price = catalogItem.Price,
					});
				}
				var basketItemsBeforeCleaning = new List<BasketItem>();
				basketItemsBeforeCleaning.AddRange(basketItems);
				var basket = new Basket
				{
					UserId = user.Id,
					BasketItems = basketItems
				};
				dbContext.Baskets.Add(basket);
				await dbContext.SaveChangesAsync();
				var resultQuantity = catalogItems.First().StorageQuantity - basketItems.First().Quantity;
				var sut = new OrderDataService(dbContext);

				// Act
				var orderId = await sut.CreateOrderAsync(basket.Id);

				// Assert
				var orderToAssert = await dbContext.Orders.FindAsync(orderId);
				var basketToAssert = await dbContext.Baskets.FindAsync(basket.Id);
				orderToAssert.Should().NotBeNull();
				orderToAssert!.OrderItems.Should().HaveCount(basketItemsBeforeCleaning.Count);
				orderToAssert.OrderItems.Should().BeEquivalentTo(basketItemsBeforeCleaning, options => options.ExcludingMissingMembers());
				orderToAssert.OrderItems.All(oi => oi.CatalogItem.StorageQuantity == resultQuantity).Should().BeTrue();
				basketToAssert!.BasketItems.Should().NotBeNull();
				basketToAssert.BasketItems.Should().BeEmpty();

			}

		}

		[Fact]
		public async Task User_gets_all_his_orders()
		{
			// Arrange
			using (var fixture = new InMemoryDatabaseFixture())
			{
				var dbContext = fixture.DbContext;
				var user = new User
				{
					UserName = Internet.UserName("test"),
					Email = Internet.Email("test"),
					PasswordHash = Lorem.Sentence(8)
				};
				user.NormalizedEmail = user.Email.ToUpper();
				user.NormalizedUserName = user.UserName.ToUpper();
				dbContext.Users.Add(user);
				var catalogItems = Helper.GenerateCatalogItems(4, 40);
				dbContext.CatalogItems.AddRange(catalogItems);
				await dbContext.SaveChangesAsync();
				var basketItems = new List<BasketItem>();
				foreach (var catalogItem in catalogItems)
				{
					basketItems.Add(new BasketItem
					{
						CatalogItemId = catalogItem.Id,
						Quantity = 10,
						Name = catalogItem.Name,
						Price = catalogItem.Price,
					});
				}
				var basket = new Basket
				{
					UserId = user.Id,
					BasketItems = basketItems
				};
				dbContext.Baskets.Add(basket);
				await dbContext.SaveChangesAsync();
				var sut = new OrderDataService(dbContext);
				await sut.CreateOrderAsync(basket.Id);
				await sut.CreateOrderAsync(basket.Id);
				await sut.CreateOrderAsync(basket.Id);

				// Act
				var resultList = await sut.GetMyOrdersAsync(user.Id);

				// Assert
				resultList.Should().HaveCount(3);
				resultList.All(list => list.OrderItems != null).Should().BeTrue();

			}

		}

		[Fact]
		public async Task User_changes_order_status()
		{
			// Arrange
			using (var fixture = new InMemoryDatabaseFixture())
			{
				var dbContext = fixture.DbContext;
				var user = new User
				{
					UserName = Internet.UserName("test"),
					Email = Internet.Email("test"),
					PasswordHash = Lorem.Sentence(8)
				};
				user.NormalizedEmail = user.Email.ToUpper();
				user.NormalizedUserName = user.UserName.ToUpper();
				dbContext.Users.Add(user);
				var catalogItems = Helper.GenerateCatalogItems(4, 40);
				dbContext.CatalogItems.AddRange(catalogItems);
				await dbContext.SaveChangesAsync();
				var basketItems = new List<BasketItem>();
				foreach (var catalogItem in catalogItems)
				{
					basketItems.Add(new BasketItem
					{
						CatalogItemId = catalogItem.Id,
						Quantity = 10,
						Name = catalogItem.Name,
						Price = catalogItem.Price,
					});
				}
				var basket = new Basket
				{
					UserId = user.Id,
					BasketItems = basketItems
				};
				dbContext.Baskets.Add(basket);
				await dbContext.SaveChangesAsync();
				var sut = new OrderDataService(dbContext);
				var orderId = await sut.CreateOrderAsync(basket.Id);

				// Act
				await sut.UpdateOrderStatusAsync(orderId, "Completed");

				// Assert
				var updatedOrder = await dbContext.Orders.FindAsync(orderId);
				updatedOrder.Should().NotBeNull();
				updatedOrder!.OrderStatus.Should().Be(OrderStatus.Completed);

			}

		}

		[Fact]
		public async Task User_gets_one_of_his_orders()
		{
			// Arrange
			using (var fixture = new InMemoryDatabaseFixture())
			{
				var dbContext = fixture.DbContext;
				var user = new User
				{
					UserName = Internet.UserName("test"),
					Email = Internet.Email("test"),
					PasswordHash = Lorem.Sentence(8)
				};
				user.NormalizedEmail = user.Email.ToUpper();
				user.NormalizedUserName = user.UserName.ToUpper();
				dbContext.Users.Add(user);
				var catalogItems = Helper.GenerateCatalogItems(4, 40);
				dbContext.CatalogItems.AddRange(catalogItems);
				await dbContext.SaveChangesAsync();
				var basketItems = new List<BasketItem>();
				foreach (var catalogItem in catalogItems)
				{
					basketItems.Add(new BasketItem
					{
						CatalogItemId = catalogItem.Id,
						Quantity = 10,
						Name = catalogItem.Name,
						Price = catalogItem.Price,
					});
				}
				var basket = new Basket
				{
					UserId = user.Id,
					BasketItems = basketItems
				};
				dbContext.Baskets.Add(basket);
				await dbContext.SaveChangesAsync();
				var sut = new OrderDataService(dbContext);
				var basketItemsBeforeCleaningIt = new List<BasketItem>();
				basketItemsBeforeCleaningIt.AddRange(basketItems);
				var orderId = await sut.CreateOrderAsync(basket.Id);

				// Act
				var order = await sut.GetOrderAsync(orderId);

				// Assert
				order.Should().NotBeNull();
				order.OrderItems.All(oi => oi != null).Should().BeTrue();
				order.OrderItems.Should().HaveCount(basketItemsBeforeCleaningIt.Count);
				order.OrderItems.Should().BeEquivalentTo(basketItemsBeforeCleaningIt, options => options.ExcludingMissingMembers());

			}

		}

		[Fact]
		public async Task Anonymous_user_tries_to_create_order()
		{
			// Arrange
			using (var fixture = new InMemoryDatabaseFixture())
			{
				var dbContext = fixture.DbContext;
				var catalogItems = Helper.GenerateCatalogItems(4, 40);
				dbContext.CatalogItems.AddRange(catalogItems);
				await dbContext.SaveChangesAsync();
				var basketItems = new List<BasketItem>();
				foreach (var catalogItem in catalogItems)
				{
					basketItems.Add(new BasketItem
					{
						CatalogItemId = catalogItem.Id,
						Quantity = 10,
						Name = catalogItem.Name,
						Price = catalogItem.Price,
					});
				}
				var basket = new Basket
				{
					UserId = null,
					BasketItems = basketItems
				};
				dbContext.Baskets.Add(basket);
				await dbContext.SaveChangesAsync();
				var sut = new OrderDataService(dbContext);

				// Act
				Func<Task> act = () => sut.CreateOrderAsync(basket.Id);

				// Assert
				await act.Should().ThrowAsync<ItemNotFoundException>().WithMessage("Anonymous user cannot order.");
			}

		}
	}
}
