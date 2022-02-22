namespace WA.Pizza.Infrastructure.DTO.Orders
{
    public record OrderItemsDTO
    {
        public int OrderId { get; init; }

        public int CatalogItemId { get; init; }

        public string Name { get; init; }

        public decimal Price { get; init; }

        public int Quantity { get; init; }
    }
}
