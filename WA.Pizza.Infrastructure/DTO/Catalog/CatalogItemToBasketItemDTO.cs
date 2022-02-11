namespace WA.Pizza.Infrastructure.DTO.Catalog
{
    public record CatalogItemToBasketItemDTO
    {
        public int CatalogItemId { get; init; }

        public int Quantity { get; init; }
    }
}
