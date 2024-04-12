using HomeAssistant.Core.Models.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Contracts
{
	public interface INotificationService
	{
		Task<int> CreateNotificationForAllUsers(string title, string description, string invokerURL, string? invokerId);

		Task<int> CreateNotificationForSpecificUsers(string title, string description, string invokerURL, string userId, string? invokerId);

		Task<IEnumerable<NotificationViewModel>> GetNotificationsForUser(string userId);

		Task<NotificationViewModel> GetNotification(int notidicationId);

		Task DismissNotification(string userId, int notificationId);
	}
}
