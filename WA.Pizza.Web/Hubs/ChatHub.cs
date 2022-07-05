using Microsoft.AspNetCore.SignalR;
using WA.Pizza.Web.Hubs.Clients;
using WA.Pizza.Web.Hubs.Models;

namespace WA.Pizza.Web.Hubs
{
	public class ChatHub : Hub<IChatClient>
	{
	}
}
