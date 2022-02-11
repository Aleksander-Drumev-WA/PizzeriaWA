namespace WA.Pizza.Infrastructure.DTO.Orders
{
    public record DetailsOrderItemDTO
    {
        public string Name { get; init; }

        public decimal Price { get; init; }

        public int Quantity { get; init; }
    }
}
