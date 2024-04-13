using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Services;
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
		private readonly IHubContext<NotificationsHub> _notificationHubContext;
		private readonly INotificationService _notificationService;

		private static List<string> connectedClients = new List<string>();

		public MessageHub(IMessageService messageService, IHubContext<NotificationsHub> notificationHubContext, INotificationService notificationService)
		{
			_messageService = messageService;
			_notificationHubContext = notificationHubContext;
			_notificationService = notificationService;
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

			if (connectedClients.Any(x => x == recipientId))
			{
				await Clients.User(recipientId).SendAsync("LoadMessage", message);
			}
			else
			{
				var notificationId = await _notificationService.CreateNotificationForAllUsers(
					"Recieved New Message",
					message,
					"/message",
					GetUserId());

				await _notificationHubContext.Clients
					.User(recipientId)
					.SendAsync("PushNotfication", await _notificationService.GetNotification(notificationId));
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
