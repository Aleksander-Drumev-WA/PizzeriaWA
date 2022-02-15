namespace WA.Pizza.Infrastructure.DTO.Catalog
{
    public record CatalogItemDTO(
        int Id,
        string Name,
        decimal Price,
        string PictureBytes,
        int StorageQuantity);
}
