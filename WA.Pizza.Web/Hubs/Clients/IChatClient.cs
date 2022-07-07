using WA.Pizza.Web.Hubs.Models;

namespace WA.Pizza.Web.Hubs.Clients
{
	public interface IChatClient
	{
		Task ReceiveMessage(ChatMessage message);
	}
}
