namespace WA.Pizza.Infrastructure.DTO.Basket
{
    public record BasketItemDTO
    {
        public int Id { get; init; }

        public int BasketId { get; init; }

        public int CatalogItemId { get; set; }

        public int Quantity { get; init; }

        public string Name { get; init; }

        public decimal Price { get; init; }

        public string PictureBytes { get; init; }

    }
}
