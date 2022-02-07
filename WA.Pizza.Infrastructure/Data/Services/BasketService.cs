using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Abstractions;
using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.Data.Services
{
    public class BasketService : IBasketService<Basket>
    {
        private readonly AppDbContext dbContext;

        public BasketService(AppDbContext dbContext)
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
                .Where(b => b.UserId == userId)
                .FirstOrDefaultAsync();

            if (basket == null)
            {
                basket = await this.dbContext
                    .Baskets
                    .Where(b => b.Id == basketId)
                    .FirstOrDefaultAsync();
            }

            return basket;
        }

        public async Task RemoveAsync(int? userId, int? basketId)
        {
            var basket  = await GetAsync(userId, basketId);

            if (basket != null)
            {
                this.dbContext.Baskets.Remove(basket);
            }
        }

        public async Task<Basket> UpdateAsync(Basket entity)
        {
            //dbContext.Entry(entity).State = EntityState.Modified;

            this.dbContext.Baskets.Update(entity);
            await dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
