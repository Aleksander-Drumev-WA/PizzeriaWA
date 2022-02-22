using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WA.Pizza.Infrastructure.Data.Services;

namespace WA.Pizza.Web.Controllers
{
    [Authorize]
    public class OrdersController : BaseController
    {
        private readonly OrderDataService _orderDataService;

        public OrdersController(OrderDataService orderDataService)
        {
            _orderDataService = orderDataService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(int basketId)
        {
            return Ok(await _orderDataService.CreateOrderAsync(basketId));
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(int userId)
        {
            return Ok(await _orderDataService.GetMyOrdersAsync(userId));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrder(int userId, string orderStatus)
        {
            return Ok(await _orderDataService.UpdateOrderStatusAsync(userId, orderStatus));
        }

        [HttpGet]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            return Ok(await _orderDataService.GetOrderAsync(orderId));
        }
    }
}
