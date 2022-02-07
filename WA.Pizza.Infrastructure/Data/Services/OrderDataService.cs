using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.Data.Services
{
    public class OrderDataService
    {
        private readonly AppDbContext dbContext;

        public OrderDataService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(Order entity)
        {
            if (entity != null)
            {
                await this.dbContext.Orders.AddAsync(entity);
                await this.dbContext.SaveChangesAsync();
            }
        }

        public async Task<Order> GetAsync(int id)
        {
            var order = await this.dbContext
                .Orders
                .Where(o => o.Id == id)
                .FirstOrDefaultAsync();

            return order;
        }

        public IQueryable<Order> GetAllAsync(int userId)
        {
            var orders = this.dbContext
                .Orders
                .Where(o => o.UserId == userId);

            return orders;
        }

        public async Task<Order> UpdateAsync(Order entity)
        {
            this.dbContext.Orders.Update(entity);
            await this.dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var order = await this.GetAsync(id);

            if (order != null)
            {
                this.dbContext.Orders.Remove(order);
                await this.dbContext.SaveChangesAsync();
            }
        }
    }
}
