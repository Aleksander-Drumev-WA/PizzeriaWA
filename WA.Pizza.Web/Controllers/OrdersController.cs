using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WA.Pizza.Infrastructure.Data.Services;

namespace WA.Pizza.Web.Controllers
{
    public class OrdersController : BaseController
    {
        private readonly OrderDataService _orderDataService;

        public OrdersController(OrderDataService orderDataService)
        {
            _orderDataService = orderDataService;
        }

        [HttpPost]
        public Task CreateOrder(int basketId)
        {
            return _orderDataService.CreateOrderAsync(basketId);
        }

        [HttpGet]
        public Task GetOrders(int userId)
        {
            return _orderDataService.GetMyOrdersAsync(userId);
        }

        [HttpPut]
        public Task UpdateOrder(int userId, string orderStatus)
        {
            return _orderDataService.UpdateOrderStatusAsync(userId, orderStatus);
        }

        [HttpGet]
        [Route("order")]
        public Task GetOrder(int orderId)
        {
            return _orderDataService.GetOrderAsync(orderId);
        }
    }
}
