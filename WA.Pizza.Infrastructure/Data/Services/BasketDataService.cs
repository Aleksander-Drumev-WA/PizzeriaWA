using Microsoft.EntityFrameworkCore;
using WA.Pizza.Core.Models;

using Mapster;
using WA.Pizza.Infrastructure.DTO.Basket;
using WA.Pizza.Infrastructure.DTO.Catalog;
using WA.Pizza.Core.Exceptions;

namespace WA.Pizza.Infrastructure.Data.Services
{
    public class BasketDataService
    {
        private readonly AppDbContext _dbContext;

        public BasketDataService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }



        public Task<List<BasketDTO>> GetBasketWithBasketItemsAsync(int basketId)
        {
            IQueryable<Basket>? basket = _dbContext
                .Baskets
                .Where(b => b.Id == basketId)
                .Include(b => b.BasketItems);

            return basket.ProjectToType<BasketDTO>().ToListAsync();
        }

        public async Task<int> AddItemToBasketAsync(CatalogItemToBasketItemRequest request)
        {
            Basket? basket = await _dbContext
                .Baskets
                .FirstOrDefaultAsync(b => b.UserId == request.UserId || b.Id == request.BasketId);

            var catalogItem = await _dbContext
                .CatalogItems
                .FirstAsync(ci => ci.Id == request.CatalogItemId);

            if (basket == null)
            {
                basket = await AssignBasketAsync(request.UserId);
            }
            if (catalogItem.StorageQuantity < request.Quantity)
            {
                throw new ArgumentException("Not enough stock in storage.");
            }

            basket.LastModifiedOn = DateTime.UtcNow;

            var basketItem = new BasketItem
            {
                BasketId = basket.Id,
                CatalogItemId = request.CatalogItemId,
                Name = request.Name,
                Price = request.Price,
                Quantity = request.Quantity,
            };

            await _dbContext.BasketItems.AddAsync(basketItem);
            await _dbContext.SaveChangesAsync();

            return basket.Id;
        }

        // I think user can only change the quantity
        public async Task<int> UpdateItemAsync(BasketItemDTO updatedBasketItem)
        {
            var localBasketItem = await _dbContext
            .BasketItems
            .Include(bi => bi.CatalogItem)
            .Include(bi => bi.Basket)
            .FirstOrDefaultAsync(bi => bi.Id == updatedBasketItem.Id);

            if (localBasketItem == null)
            {
                throw new ItemNotFoundException("Basket item cannot be found or deleted.");
            }
            if (localBasketItem.CatalogItem.StorageQuantity < updatedBasketItem.Quantity)
            {
                throw new ArgumentException("Not enough stock in storage.");
            }

            // this changes updatedBasketItem props
            // localBasketItem.Adapt(updatedBasketItem);

            updatedBasketItem.Adapt(localBasketItem);
            localBasketItem.Basket.LastModifiedOn = DateTime.UtcNow;

            localBasketItem.CatalogItem.StorageQuantity -= updatedBasketItem.Quantity;
            await _dbContext.SaveChangesAsync();


            return localBasketItem.Id;
        }

        public async Task RemoveBasketItem(int basketItemId)
        {
            BasketItem? basketItem = await _dbContext
                .BasketItems
                .Where(bi => bi.Id == basketItemId)
                .FirstOrDefaultAsync();

            if (basketItem != null)
            {
                _dbContext.BasketItems.Remove(basketItem);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task<Basket> AssignBasketAsync(int? userId)
        {
            Basket? basket = new Basket();

            if (userId != null)
            {
                basket.UserId = userId.Value;
            }

            await _dbContext.Baskets.AddAsync(basket);
            await _dbContext.SaveChangesAsync();

            return basket;
        }
    }
}
