namespace WA.Pizza.Infrastructure.DTO.Catalog
{
    public record GetCatalogItemsDTO
    {
        public string Name { get; init; }

        public decimal Price { get; init; }

        public string PictureBytes { get; init; }
    }
}
