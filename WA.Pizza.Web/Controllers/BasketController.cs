using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WA.Pizza.Infrastructure.BasketHandlers;
using WA.Pizza.Infrastructure.Data.Services;
using WA.Pizza.Infrastructure.DTO.Basket;
using WA.Pizza.Infrastructure.DTO.Catalog;

namespace WA.Pizza.Web.Controllers
{
    [AllowAnonymous]
    public class BasketController : BaseController
    {
        private readonly IMediator _mediator;

        public BasketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{basketId}")]
        public Task<List<BasketDTO>> GetBasket(int basketId)
        {
            return _mediator.Send(new GetBasketQuery(basketId));
        }

        [HttpPost]
        public Task<int> AddItem(AddBasketItemCommand request)
        {
            return _mediator.Send(request);
        }

        [HttpPut]
        public Task<int> UpdateItem(UpdateBasketCommand updatedBasketItem)
        {
            return _mediator.Send(updatedBasketItem);
        }

        [HttpDelete("{basketItemId}")]
        public async Task<IActionResult> DeleteItem(int basketItemId)
        {
            await _mediator.Send(new DeleteBasketItemCommand(basketItemId));
            return NoContent();
        }
    }
}
