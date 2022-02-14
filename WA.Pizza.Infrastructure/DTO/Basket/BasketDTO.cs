namespace WA.Pizza.Infrastructure.DTO.Basket
{
    public record BasketDTO
    {
        public ICollection<GetBasketItemsDTO> BasketItems { get; init; }

		public string UserName { get; init; }
	}
}
