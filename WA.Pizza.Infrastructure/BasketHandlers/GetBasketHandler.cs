using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WA.Pizza.Infrastructure.Data;
using WA.Pizza.Infrastructure.DTO.Basket;

namespace WA.Pizza.Infrastructure.BasketHandlers
{
	public record GetBasketQuery(int BasketId) : IRequest<List<BasketDTO>>;

	public class GetBasketHandler : IRequestHandler<GetBasketQuery, List<BasketDTO>>
	{
		private readonly AppDbContext _dbContext;

		public GetBasketHandler(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public Task<List<BasketDTO>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
		{
			return _dbContext.Baskets
				.Where(b => b.Id == request.BasketId)
				.Include(b => b.BasketItems)
				.AsNoTracking()
				.ProjectToType<BasketDTO>()
				.ToListAsync(cancellationToken);
		}
	}
}
