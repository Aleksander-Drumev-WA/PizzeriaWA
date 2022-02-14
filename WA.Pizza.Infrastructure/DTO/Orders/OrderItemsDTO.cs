namespace WA.Pizza.Infrastructure.DTO.Orders
{
    public record OrderItemsDTO
    {
        public string Name { get; init; }

        public decimal Price { get; init; }

        public int Quantity { get; init; }
    }
}
