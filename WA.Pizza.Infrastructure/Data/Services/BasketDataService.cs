using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.Data.Services
{
    public class BasketDataService
    {
        private readonly AppDbContext dbContext;

        public BasketDataService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(int? userId)
        {
            var basket = new Basket();

            if (userId != null)
            {
                basket.UserId = userId.Value;
            }

            await this.dbContext.Baskets.AddAsync(basket);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<Basket> GetAsync(int? userId, int? basketId)
        {
            var basket = await this.dbContext
                .Baskets
                .Where(b => b.UserId == userId || b.Id == basketId)
                .FirstOrDefaultAsync();

            return basket;
        }

        public async Task<Basket> UpdateAsync(Basket entity)
        {
            this.dbContext.Baskets.Update(entity);
            await dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
