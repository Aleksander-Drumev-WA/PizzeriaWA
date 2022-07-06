using WA.Pizza.Core.Abstractions;
using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.Abstractions
{
	public class BasketNotModifiedForWeekSpecification : ExpressionSpecification<Basket>
	{
		public BasketNotModifiedForWeekSpecification() : base(basket => basket.BasketItems.Any() &&
															  basket.LastModifiedOn.HasValue &&
															  DateTime.UtcNow.Subtract(basket.LastModifiedOn.Value) >= TimeSpan.FromDays(7))
		{

		}
	}
}
