using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WA.Pizza.Web.Hubs;
using WA.Pizza.Web.Hubs.Clients;
using WA.Pizza.Web.Hubs.Models;

namespace WA.Pizza.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub, IChatClient> _chatHub;

        public ChatController(IHubContext<ChatHub, IChatClient> chatHub)
        {
            _chatHub = chatHub;
        }

        [HttpPost("messages")]
        public async Task Post(ChatMessage message)
        {
            // run some logic...

            await _chatHub.Clients.All.ReceiveMessage(message);
        }
    }
}
