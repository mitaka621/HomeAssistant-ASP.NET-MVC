using HomeAssistant.Core.Models.Notification;

namespace HomeAssistant.Core.Contracts
{
	public interface INotificationService
	{
		Task<int> CreateNotificationForAllUsers(string title, string description, string invokerURL, string? invokerId);

        Task<int> CreateNotificationForAllUsersExceptOne(string title, string description,string exceptUserId, string invokerURL, string? invokerId);

        Task<int> CreateNotificationForSpecificUser(string title, string description, string invokerURL, string userId, string? invokerId);

		Task<NotificationsWithPfpModel> GetNotificationsForUser(string userId,int take = 10, int skip = 0);

		Task<NotificationViewModel> GetNotification(int notidicationId);

		Task DismissNotification(string userId, int notificationId);

		Task DismissAllNotificationsForUser(string userId);

		Task<IEnumerable<NotificationViewModel>> GetTop20ProductRelatedNotification();

		Task SubscribeUserForPush(PushNotificationRegistrationModel model,string userId);

		Task<bool> PushNotificationForUser(string userId,string title, string body, string url,string? iconUrl);

		Task<bool> PushNotificationForAllUsers(string title, string body, string url, string? iconUrl);

		Task<bool> PushNotificationForAllUsersExcept(string userIdToExclude, string title, string body, string url, string? iconUrl);
	}
}
