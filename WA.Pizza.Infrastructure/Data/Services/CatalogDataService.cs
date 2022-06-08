using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.DTO.Catalog;
using Mapster;
using WA.Pizza.Core.Exceptions;

namespace WA.Pizza.Infrastructure.Data.Services
{
    public class CatalogDataService
    {
        private readonly AppDbContext _dbContext;

        public CatalogDataService(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<int> AddOrUpdateAsync(CatalogItemDTO dto)
        {
            var catalogItem = _dbContext
                              .CatalogItems
                              .First(ci => ci.Id == dto.Id);

            if (catalogItem == null)
            {
                dto.Adapt(catalogItem);
                await _dbContext.CatalogItems.AddAsync(catalogItem);
            }
            else
            {
                dto.Adapt(catalogItem);
            }

            await _dbContext.SaveChangesAsync();

            return catalogItem.Id;
        }

        // functionality for pagination?
        public Task<List<ListCatalogItemsDTO>> GetAllAsync()
        {
            var catalogItems = _dbContext
                .CatalogItems;

            return catalogItems.ProjectToType<ListCatalogItemsDTO>().ToListAsync();
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
                throw new ItemNotFoundException("Catalog item cannot be found or it is deleted.");
            }

            _dbContext.CatalogItems.Remove(catalogItem);
            await _dbContext.SaveChangesAsync();
        }
    }
}
