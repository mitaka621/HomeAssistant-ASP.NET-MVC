using HomeAssistant.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Security.Claims;

namespace HomeAssistant.Hubs
{
	[Authorize(Roles = "StandardUser")]
	public class MessageHub : Hub
	{
		private readonly IMessageService _messageService;
		private static List<string> connectedClients = new List<string>();

		public MessageHub(IMessageService messageService)
		{
			_messageService = messageService;
		}

		public override Task OnConnectedAsync()
		{
			connectedClients.Add(GetUserId());
			return base.OnConnectedAsync();
		}

		public async Task SendMessage(int chatRoomId, string recipientId, string message)
		{
			await _messageService
					.SendMessage(chatRoomId, GetUserId(), recipientId, message);

            if (connectedClients.Any(x=>x==recipientId))
            {
				await Clients.User(recipientId).SendAsync("LoadMessage", message);
			}         
		}

		public override Task OnDisconnectedAsync(Exception? exception)
		{
			connectedClients.Remove(GetUserId());
			return base.OnDisconnectedAsync(exception);
		}

		private string GetUserId()
		{
			return Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
