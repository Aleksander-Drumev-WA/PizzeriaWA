using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.DTO.Orders
{
    public record ListOrdersDTO
    {
        public decimal Total { get; init; }

        public OrderStatus Status { get; init; }

        public ICollection<OrderItemsDTO> OrderItems { get; init; }
    }
}
