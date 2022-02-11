using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.DTO.Orders;

using Mapster;

namespace WA.Pizza.Infrastructure.Data.Services
{
    public class OrderDataService
    {
        private readonly AppDbContext _dbContext;

        public OrderDataService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateOrderAsync(int basketId)
        {
            decimal total = 0.0m;

            var basket = await _dbContext
                .Baskets
                .Include(b => b.BasketItems)
                .ThenInclude(bi => bi.CatalogItem)
                .FirstAsync(b => b.Id == basketId);

            var catalogItems = basket.BasketItems.Select(bi => bi.CatalogItem).ToList();
            foreach (var currentCatalogItem in catalogItems)
            {
                var currentBasketItem = basket.BasketItems.First(bi => bi.CatalogItemId == currentCatalogItem.Id);
                if (currentBasketItem.Quantity <= 0)
                {
                    throw new ArgumentException("Cannot puchase item with 0 or less quantity.");
                }
                else if (currentBasketItem.Quantity > currentCatalogItem.StorageQuantity)
                {
                    throw new ArgumentException("Not enough catalog items in storage.");
                }

                total += currentBasketItem.Quantity * currentCatalogItem.Price;
            }

            var order = new Order
            {
                CreatedOn = DateTime.UtcNow,
                Total = total,
                UserId = basket.UserId.Value,
                OrderStatus = OrderStatus.New
            };


            await _dbContext.Orders.AddAsync(order);

            foreach (var currentCatalogItem in catalogItems)
            {
                var quantity = currentCatalogItem.BasketItems.First(bi => bi.CatalogItemId == currentCatalogItem.Id).Quantity;

                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    CatalogItemId = currentCatalogItem.Id,
                    Name = currentCatalogItem.Name,
                    Price = currentCatalogItem.Price,
                    Quantity = quantity
                };

                await _dbContext.OrderItems.AddAsync(orderItem);
            }

            return order.Id;
        }



        public ICollection<ListOrdersDTO> GetMyOrdersAsync(int userId)
        {
            var orders = _dbContext
            .Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems);

            return orders.ProjectToType<ListOrdersDTO>().ToList();
        }

        public async Task<int> UpdateOrderStatusAsync(int orderId, OrderStatus orderStatus)
        {
            var order = await _dbContext
                .Orders
                .FirstAsync(o => o.Id == orderId);

            order.OrderStatus = orderStatus;

            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();

            return order.Id;
        }

        public async Task<GetSingleOrderDTO> GetOrderAsync(int orderId)
        {
            var order = await _dbContext
                .Orders
                .Include(o => o.OrderItems)
                .FirstAsync(o => o.Id == orderId);

            // Reusing GetSingleOrderDTO
            return order.Adapt<GetSingleOrderDTO>();
        }
    }
}
