using HomeAssistant.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace HomeAssistant.Hubs
{
	[Authorize(Roles ="StandardUser")]
	public class NotificationsHub:Hub
	{
		public INotificationService _notificationService;

        public NotificationsHub(INotificationService notificationService)
        {
			_notificationService=notificationService;

		}
        public async Task MarkAsDismissed(int notificationId)
		{
			await _notificationService.DismissNotification(Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, notificationId);
		}
	}
}
