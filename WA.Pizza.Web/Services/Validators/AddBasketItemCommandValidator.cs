using FluentValidation;
using WA.Pizza.Infrastructure.BasketHandlers;

namespace WA.Pizza.Web.Services.Validators
{
    public class AddBasketItemCommandValidator : AbstractValidator<AddBasketItemCommand>
    {
        public AddBasketItemCommandValidator()
        {
            RuleFor(x => x.CatalogItemId).NotNull();
            RuleFor(x => x.Quantity).InclusiveBetween(1, int.MaxValue);
            RuleFor(x => x.Name).Length(1, 150);
            RuleFor(x => x.Price).ScalePrecision(18, 2);
        }
    }
}
