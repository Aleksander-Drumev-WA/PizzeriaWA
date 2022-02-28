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
        public Task GetBasket(int basketId)
        {
            return _basketDataService.GetBasketWithBasketItemsAsync(basketId);
        }

        [HttpPost]
        public Task AddItem(CatalogItemToBasketItemRequest request)
        {
            return _basketDataService.AddItemToBasketAsync(request);
        }

        [HttpPut]
        public Task UpdateItem(BasketItemDTO updatedBasketItem)
        {
            return _basketDataService.UpdateItemAsync(updatedBasketItem);
        }

        [HttpDelete]
        public Task DeleteItem(int basketItemId)
        {
            return _basketDataService.RemoveBasketItem(basketItemId);
        }
    }
}
