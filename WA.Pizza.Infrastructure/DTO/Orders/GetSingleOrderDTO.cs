using WA.Pizza.Core.Models;

namespace WA.Pizza.Infrastructure.DTO.Orders
{
    public record GetSingleOrderDTO
    {
        public decimal Total { get; init; }

        public OrderStatus Status { get; init; }

        public DetailsOrderItemDTO OrderItems { get; init; }
    }
}
