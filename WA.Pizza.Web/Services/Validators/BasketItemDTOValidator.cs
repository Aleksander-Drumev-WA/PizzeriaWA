using FluentValidation;
using WA.Pizza.Infrastructure.DTO.Basket;

namespace WA.Pizza.Web.Services.Validators
{
    public class BasketItemDTOValidator : AbstractValidator<BasketItemDTO>
    {
        public BasketItemDTOValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.BasketId).NotNull();
            RuleFor(x => x.CatalogItemId).NotNull();
            RuleFor(x => x.Quantity).InclusiveBetween(1, int.MaxValue);
            RuleFor(x => x.Name).Length(1, 150);
            RuleFor(x => x.Price).ScalePrecision(18, 2);
            RuleFor(x => x.PictureBytes).Length(1, 30000);
        }
    }
}
