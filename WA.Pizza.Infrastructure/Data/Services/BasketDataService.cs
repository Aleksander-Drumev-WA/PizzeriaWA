using Microsoft.EntityFrameworkCore;
using WA.Pizza.Core.Models;

using Mapster;
using WA.Pizza.Infrastructure.DTO.Basket;
using WA.Pizza.Infrastructure.DTO.Catalog;

namespace WA.Pizza.Infrastructure.Data.Services
{
    public class BasketDataService
    {
        private readonly AppDbContext _dbContext;

        public BasketDataService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }



        public ICollection<GetBasketDTO> GetBasketItems(int basketId)
        {
            IQueryable<Basket>? basket = _dbContext
                .Baskets
                .Where(b => b.Id == basketId)
                .Include(b => b.BasketItems)
                .ThenInclude(bi => bi.CatalogItem);

            return basket.ProjectToType<GetBasketDTO>().ToList();
        }

        public async Task<int> AddItemToBasketAsync(int? userId, int? basketId, CatalogItemToBasketItemDTO dto)
        {
            Basket? basket = await _dbContext
                .Baskets
                .FirstAsync(b => b.UserId == userId || b.Id == basketId);

            if (basket == null)
            {
                basket = await AssignBasketAsync(userId);
            }

            BasketItem? basketItem = new BasketItem
            {
                BasketId = basket.Id,
                Quantity = dto.Quantity,
                CatalogItemId = dto.CatalogItemId
            };

            await _dbContext.BasketItems.AddAsync(basketItem);
            await _dbContext.SaveChangesAsync();

            return basketItem.Id;
        }

        // I think user can only change the quantity
        public async Task<int> UpdateItemAsync(UpdateBasketItemDTO updatedBasketItem)
        {
            BasketItem? localBasketItem = await _dbContext
                .BasketItems
                .FirstAsync(bi => bi.Id == updatedBasketItem.Id);

            if (localBasketItem == null)
            {
                throw new ArgumentNullException("Basket item cannot be found or deleted.");
            }

            localBasketItem.Quantity = updatedBasketItem.Quantity;
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
                await _dbContext.Baskets.AddAsync(basket);
                await _dbContext.SaveChangesAsync();
            }


            return basket;
        }
    }
}
