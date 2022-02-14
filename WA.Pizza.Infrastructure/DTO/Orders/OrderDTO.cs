using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.DTO.Orders
{
    public record OrderDTO
    {
		public int Id { get; init; }

		public decimal Total { get; init; }

        public OrderStatus Status { get; init; }

        public List<OrderItemsDTO> OrderItems { get; init; }
    }
}
