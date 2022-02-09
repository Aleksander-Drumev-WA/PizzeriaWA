using Microsoft.EntityFrameworkCore;
using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.Data.Services
{
    public class BasketDataService
    {
        private readonly AppDbContext _dbContext;

        public BasketDataService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }



        public IQueryable<BasketItem> GetBasketItems(int basketId)
        {
            IQueryable<BasketItem>? basket = _dbContext
                .BasketItems
                .Where(bi => bi.BasketId == basketId)
                .Include(bi => bi.CatalogItem)
                .Select(bi => new BasketItem
                {
                    Id = bi.Id,
                    Quantity = bi.Quantity,
                    CatalogItem = new CatalogItem
                    {
                        Name = bi.CatalogItem.Name,
                        Price = bi.CatalogItem.Price,
                        PictureBytes = bi.CatalogItem.PictureBytes
                    }
                });

            return basket;
        }

        public async Task<BasketItem> AddItemToBasketAsync(int? userId, int? basketId, CatalogItem item)
        {
            Basket? basket = await _dbContext
                .Baskets
                .Where(b => b.UserId == userId || b.Id == basketId)
                .FirstOrDefaultAsync();

            if (basket == null)
            {
                basket = await AssignBasketAsync(userId);
            }

            BasketItem? basketItem = new BasketItem
            {
                BasketId = basket.Id,
                // I think I need DTO for this to set it properly (quantity)
                Quantity = 1,
                CatalogItemId = item.Id
            };

            await _dbContext.BasketItems.AddAsync(basketItem);
            await _dbContext.SaveChangesAsync();

            return basketItem;
        }

        public async Task<int> UpdateItemAsync(BasketItem updatedBasketItem)
        {
            BasketItem? localBasketItem = await _dbContext
                .BasketItems
                .FirstAsync(bi => bi.Id == updatedBasketItem.Id);

            if (localBasketItem == null)
            {
                throw new ArgumentNullException("Basket item cannot be found or deleted.");
            }

            localBasketItem = updatedBasketItem;
            _dbContext.BasketItems.Update(localBasketItem);
            await _dbContext.SaveChangesAsync();


            return localBasketItem.Id;
        }

        public async Task RemoveBasketItemAsync(int basketItemId)
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
