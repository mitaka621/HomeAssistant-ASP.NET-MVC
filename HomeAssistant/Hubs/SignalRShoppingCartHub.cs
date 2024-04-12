using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HomeAssistant.Hubs
{
	[Authorize(Roles = "StandardUser")]
	public class SignalRShoppingCartHub:Hub
	{
	}
}
