using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WA.Pizza.Infrastructure.Data.Services;
using WA.Pizza.Infrastructure.DTO.Basket;
using WA.Pizza.Infrastructure.DTO.Catalog;

namespace WA.Pizza.Web.Controllers
{
    [AllowAnonymous]
    public class BasketController : BaseController
    {
        private readonly BasketDataService _basketDataService;

        public BasketController(BasketDataService basketDataService)
        {
            _basketDataService = basketDataService;
        }

        [HttpGet]
        [Route("{basketId}")]
        public async Task<IActionResult> GetBasket(int basketId)
        {
            return Ok(await _basketDataService.GetBasketWithBasketItemsAsync(basketId));
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(CatalogItemToBasketItemRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(request);
            }

            return Ok(await _basketDataService.AddItemToBasketAsync(request));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateItem(BasketItemDTO updatedBasketItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(updatedBasketItem);
            }

            return Ok(await _basketDataService.UpdateItemAsync(updatedBasketItem));
        }

        [HttpDelete]
        public IActionResult DeleteItem(int basketItemId)
        {
            return Ok(_basketDataService.RemoveBasketItem(basketItemId));
        }
    }
}
