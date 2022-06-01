using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.DTO.Orders;

using Mapster;
using WA.Pizza.Core.Exceptions;

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

            if (basket.UserId == null)
            {
                throw new ItemNotFoundException(message: "Anonymous user cannot order.");
            }

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

                currentCatalogItem.StorageQuantity -= currentBasketItem.Quantity;
                total += currentBasketItem.Quantity * currentCatalogItem.Price;
            }

            var orderItems = new List<OrderItem>();
            foreach (var currentBasketItem in basket.BasketItems)
            {
                var orderItem = new OrderItem
                {
                    CatalogItemId = currentBasketItem.Id,
                    Name = currentBasketItem.Name,
                    Price = currentBasketItem.Price,
                    Quantity = currentBasketItem.Quantity
                };
                orderItems.Add(orderItem);
            }
            _dbContext.BasketItems.RemoveRange(basket.BasketItems);
            _dbContext.OrderItems.AddRange(orderItems);


            var order = new Order
            {
                CreatedOn = DateTime.UtcNow,
                Total = total,
                UserId = basket.UserId.Value,
                OrderStatus = OrderStatus.New,
                OrderItems = orderItems
            };
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            return order.Id;
        }



        public Task<List<ListOrdersDTO>> GetMyOrdersAsync(int userId)
        {
            var orders = _dbContext
            .Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems);

            return orders.ProjectToType<ListOrdersDTO>().ToListAsync();
        }

        public async Task<int> UpdateOrderStatusAsync(int orderId, string orderStatus)
        {
            var order = await _dbContext
                .Orders
                .FirstAsync(o => o.Id == orderId);

            order.OrderStatus = (OrderStatus)Enum.Parse(typeof(OrderStatus), orderStatus);

            await _dbContext.SaveChangesAsync();

            return order.Id;
        }

        public async Task<OrderDTO> GetOrderAsync(int orderId)
        {
            var order = await _dbContext
                .Orders
                .Include(o => o.OrderItems)
                .ProjectToType<OrderDTO>()
                .FirstAsync(o => o.Id == orderId);

            return order;
        }
    }
}
