namespace WA.Pizza.Infrastructure.DTO.Basket
{
    public record BasketDTO
    {
        public ICollection<BasketItemDTO> BasketItems { get; init; }

		public string UserName { get; init; }
	}
}
