namespace WA.Pizza.Infrastructure.DTO.Basket
{
    public record UpdateBasketItemDTO
    {
        public int Id { get; init; }

        public int Quantity { get; init; }
    }
}
