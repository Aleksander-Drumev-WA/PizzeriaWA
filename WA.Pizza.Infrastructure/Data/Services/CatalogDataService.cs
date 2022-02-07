using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.Data.Services
{
    public class CatalogDataService
    {
        private readonly AppDbContext dbContext;

        public CatalogDataService(AppDbContext dbContext)
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

        public IQueryable<CatalogItem> GetAllAsync()
        {
            var catalogItems = this.dbContext
                .CatalogItems;

            return catalogItems;
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
