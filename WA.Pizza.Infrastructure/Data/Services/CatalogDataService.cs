using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.DTO.Catalog;
using Mapster;

namespace WA.Pizza.Infrastructure.Data.Services
{
    public class CatalogDataService
    {
        private readonly AppDbContext _dbContext;

        public CatalogDataService(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<int> AddAsync(CatalogItemDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException("No data provided for item creation.");
            }

            var catalogItem = dto.Adapt<CatalogItem>();

            await _dbContext.CatalogItems.AddAsync(catalogItem);
            await _dbContext.SaveChangesAsync();

            return catalogItem.Id;
        }

        // functionality for pagination?
        public ICollection<ListCatalogItemsDTO> GetAll()
        {
            var catalogItems = _dbContext
                .CatalogItems;

            return catalogItems.ProjectToType<ListCatalogItemsDTO>().ToList();
        }

        public async Task<CatalogItemDTO> GetOneCatalogItemAsync(int catalogItemId)
        {
            var catalogItem = await _dbContext
                .CatalogItems
                .ProjectToType<CatalogItemDTO>()
                .FirstAsync(ci => ci.Id == catalogItemId);

            return catalogItem;
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

        public async Task<int> UpdateAsync(CatalogItemDTO updatedCatalogItem)
        {
            var catalogItem = updatedCatalogItem.Adapt<CatalogItem>();

            if (catalogItem == null)
            {
                throw new ArgumentNullException("Catalog item cannot be found or it is deleted.");
            }

            _dbContext.CatalogItems.Update(catalogItem);
            await _dbContext.SaveChangesAsync();

            return catalogItem.Id;
        }
    }
}
