using MediatR;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.Data;

namespace WA.Pizza.Infrastructure.BasketHandlers
{
	public record DeleteBasketItemCommand(int BasketItemId) : IRequest;

	public class DeleteBasketItemHandler : AsyncRequestHandler<DeleteBasketItemCommand>
	{
		private readonly AppDbContext _dbContext;

		public DeleteBasketItemHandler(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		protected override Task Handle(DeleteBasketItemCommand request, CancellationToken cancellationToken)
		{
			_dbContext.BasketItems.Remove(new BasketItem() { Id = request.BasketItemId });
			return _dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
