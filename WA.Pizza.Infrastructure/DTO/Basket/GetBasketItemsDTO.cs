using WA.Pizza.Infrastructure.DTO.Catalog;

namespace WA.Pizza.Infrastructure.DTO.Basket
{
    public record GetBasketItemsDTO
    {
        public int Quantity { get; init; }

        public GetCatalogItemsDTO CatalogItem { get; init; }


    }
}
