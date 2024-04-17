using HomeAssistant.Core.Models.Notification;

namespace HomeAssistant.Core.Contracts
{
	public interface INotificationService
	{
		Task<int> CreateNotificationForAllUsers(string title, string description, string invokerURL, string? invokerId);

        Task<int> CreateNotificationForAllUsersExceptOne(string title, string description,string exceptUserId, string invokerURL, string? invokerId);

        Task<int> CreateNotificationForSpecificUser(string title, string description, string invokerURL, string userId, string? invokerId);

		Task<IEnumerable<NotificationViewModel>> GetNotificationsForUser(string userId);

		Task<NotificationViewModel> GetNotification(int notidicationId);

		Task DismissNotification(string userId, int notificationId);

		Task<IEnumerable<NotificationViewModel>> GetTop20ProductRelatedNotification();
	}
}
