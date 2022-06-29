using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WA.Pizza.Core.Exceptions;
using WA.Pizza.Infrastructure.Data;

namespace WA.Pizza.Infrastructure.BasketHandlers
{
	public record UpdateBasketCommand(
		int Id,
		int BasketId,
		int CatalogItemId,
		int Quantity,
		string Name,
		decimal Price,
		string PictureBytes) : IRequest<int>;

	public class UpdateBasketHandler : IRequestHandler<UpdateBasketCommand, int>
	{
		private readonly AppDbContext _dbContext;

		public UpdateBasketHandler(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<int> Handle(UpdateBasketCommand request, CancellationToken cancellationToken)
		{
            var localBasketItem = await _dbContext
            .BasketItems
            .Include(bi => bi.CatalogItem)
            .Include(bi => bi.Basket)
            .FirstOrDefaultAsync(bi => bi.Id == request.Id);

            if (localBasketItem == null)
            {
                throw new ItemNotFoundException("Basket item cannot be found or deleted.");
            }
            if (localBasketItem.CatalogItem.StorageQuantity < request.Quantity)
            {
                throw new ArgumentException("Not enough stock in storage.");
            }

            request.Adapt(localBasketItem);
            localBasketItem.Basket.LastModifiedOn = DateTime.UtcNow;

            localBasketItem.CatalogItem.StorageQuantity -= request.Quantity;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return localBasketItem.Id;
        }
	}
}
