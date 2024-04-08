using Microsoft.AspNetCore.SignalR;

namespace HomeAssistant.Hubs
{
	public class SignalRShoppingCartHub:Hub
	{
		public async Task SendMessage(string userId, int progress)
		{
			await Clients.All.SendAsync("GetShoppingCartUpdate",userId, progress);
		}
	}
}
