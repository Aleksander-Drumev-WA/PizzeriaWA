using FluentValidation;
using WA.Pizza.Infrastructure.DTO.Catalog;

namespace WA.Pizza.Web.Services.Validators
{
    public class CatalogItemToBasketItemRequestValidator : AbstractValidator<CatalogItemToBasketItemRequest>
    {
        public CatalogItemToBasketItemRequestValidator()
        {
            RuleFor(x => x.CatalogItemId).NotNull();
            RuleFor(x => x.Quantity).InclusiveBetween(1, int.MaxValue);
            RuleFor(x => x.Name).Length(1, 150);
            RuleFor(x => x.Price).ScalePrecision(18, 2);
        }
    }
}
