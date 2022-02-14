using WA.Pizza.Infrastructure.DTO.Catalog;

namespace WA.Pizza.Infrastructure.DTO.Basket
{
    public record GetBasketItemsDTO
    {
        public int Quantity { get; init; }

        public string Name { get; init; }

        public decimal Price { get; init; }

        public string PictureBytes { get; init; }

    }
}
