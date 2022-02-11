namespace WA.Pizza.Infrastructure.DTO.Catalog
{
    public record UpdateCatalogItemDTO
    {
        public int Id { get; init; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string PictureBytes { get; set; }

        public int StorageQuantity { get; set; }
    }
}
