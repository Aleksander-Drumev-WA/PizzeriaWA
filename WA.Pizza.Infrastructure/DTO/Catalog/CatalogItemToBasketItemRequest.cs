namespace WA.Pizza.Infrastructure.DTO.Catalog
{
    public record CatalogItemToBasketItemRequest
    {
		public int? BasketId { get; init; }

		public int CatalogItemId { get; init; }

        public int Quantity { get; init; }
    }
}
