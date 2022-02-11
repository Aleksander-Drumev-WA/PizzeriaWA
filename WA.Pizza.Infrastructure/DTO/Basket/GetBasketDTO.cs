namespace WA.Pizza.Infrastructure.DTO.Basket
{
    public record GetBasketDTO
    {
        public ICollection<GetBasketItemsDTO> BasketItems { get; init; }

    }
}
