using MediatR;
using Microsoft.EntityFrameworkCore;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.Data;

namespace WA.Pizza.Infrastructure.BasketHandlers
{
	public record AddBasketItemCommand(
		int? BasketId,
		int? UserId,
		int CatalogItemId,
		int Quantity,
		string Name,
		decimal Price) : IRequest<int>;

	public class AddBasketItemHandler : IRequestHandler<AddBasketItemCommand, int>
	{
		private readonly AppDbContext _dbContext;

		public AddBasketItemHandler(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<int> Handle(AddBasketItemCommand request, CancellationToken cancellationToken)
		{
            var basket = await _dbContext
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

            var entry = _dbContext.BasketItems.Add(basketItem);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entry.Entity.Id;
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
