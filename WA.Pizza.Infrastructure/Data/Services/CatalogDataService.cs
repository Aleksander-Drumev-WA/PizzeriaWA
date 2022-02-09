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
        private readonly AppDbContext _dbContext;

        public CatalogDataService(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task AddAsync(CatalogItem entity)
        {
            if (entity != null)
            {
                await _dbContext.CatalogItems.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
            }
        }

        // functionality for pagination?
        public IQueryable<CatalogItem> GetAllAsync()
        {
            var catalogItems = _dbContext
                .CatalogItems;

            return catalogItems;
        }

        public async Task RemoveAsync(int Id)
        {
            var catalogItem = await _dbContext
                .CatalogItems
                .FirstAsync(ci => ci.Id == Id);

            if (catalogItem == null)
            {
                throw new ArgumentNullException("Catalog item cannot be found or it is deleted.");
            }

            _dbContext.CatalogItems.Remove(catalogItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(CatalogItem updatedCatalogItem)
        {
            var catalogItem = await _dbContext
                .CatalogItems
                .FirstOrDefaultAsync(ci => ci.Id == updatedCatalogItem.Id);

            if (catalogItem == null)
            {
                throw new ArgumentNullException("Catalog item cannot be found or it is deleted.");
            }

            catalogItem = updatedCatalogItem;
            _dbContext.CatalogItems.Update(catalogItem);
            await _dbContext.SaveChangesAsync();

            return updatedCatalogItem.Id;
        }
    }
}
