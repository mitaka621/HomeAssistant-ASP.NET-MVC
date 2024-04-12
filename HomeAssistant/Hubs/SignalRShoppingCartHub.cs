using HomeAssistant.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace HomeAssistant.Hubs
{
	[Authorize(Roles = "StandardUser")]
	public class SignalRShoppingCartHub:Hub
	{

	}
}
