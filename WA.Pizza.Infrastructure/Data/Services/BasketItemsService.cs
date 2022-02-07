using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.Data.Services
{
    public class BasketItemsService
    {
        private readonly AppDbContext dbContext;

        public BasketItemsService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(BasketItem entity)
        {
            if (entity != null)
            {
                await this.dbContext.BasketItems.AddAsync(entity);
                await this.dbContext.SaveChangesAsync();
            }
        }

        public async Task<IQueryable<BasketItem>> GetAllAsync()
        {
            var basketItems = await this.dbContext
                .BasketItems
                .ToListAsync();

            return basketItems.AsQueryable();
        }

        public async Task<BasketItem> GetAsync(int Id)
        {
            var basketItem = await this.dbContext
                .BasketItems
                .Where(bi => bi.Id == Id)
                .FirstOrDefaultAsync();

            return basketItem;
        }

        public async Task RemoveAsync(int Id)
        {
            var basketItem = await this.GetAsync(Id);

            if (basketItem != null)
            {
                this.dbContext.BasketItems.Remove(basketItem);
                await this.dbContext.SaveChangesAsync();
            }
        }

        public async Task<BasketItem> UpdateAsync(BasketItem entity)
        {
            this.dbContext.BasketItems.Update(entity);
            await this.dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
