using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.Data.Services
{
    public class CatalogItemsService
    {
        private readonly AppDbContext dbContext;

        public CatalogItemsService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(CatalogItem entity)
        {
            if (entity != null)
            {
                await this.dbContext.CatalogItems.AddAsync(entity);
                await this.dbContext.SaveChangesAsync();
            }
        }

        public async Task<IQueryable<CatalogItem>> GetAllAsync()
        {
            var catalogItems = await this.dbContext
                .CatalogItems
                .ToListAsync();

            return catalogItems.AsQueryable();
        }

        public async Task<CatalogItem> GetAsync(int Id)
        {
            var catalogItem = await this.dbContext
                .CatalogItems
                .Where(ci => ci.Id == Id)
                .FirstOrDefaultAsync();

            // If Id is null throw exception??

            return catalogItem;
        }

        public async Task RemoveAsync(int Id)
        {
            var catalogItem = await this.GetAsync(Id);

            if (catalogItem != null)
            {
                this.dbContext.CatalogItems.Remove(catalogItem);
                await this.dbContext.SaveChangesAsync();
            }
        }

        public async Task<CatalogItem> UpdateAsync(CatalogItem entity)
        {
            this.dbContext.CatalogItems.Update(entity);
            await this.dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
