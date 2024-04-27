﻿using HomeAssistant.Core.Models.Notification;

namespace HomeAssistant.Core.Contracts
{
	public interface INotificationService
	{
		Task<int> CreateNotificationForAllUsers(string title, string description, string invokerURL, string? invokerId);

        Task<int> CreateNotificationForAllUsersExceptOne(string title, string description,string exceptUserId, string invokerURL, string? invokerId);

        Task<int> CreateNotificationForSpecificUser(string title, string description, string invokerURL, string userId, string? invokerId);

		Task<IEnumerable<NotificationViewModel>> GetNotificationsForUser(string userId,int take = 30, int skip = 0);

		Task<NotificationViewModel> GetNotification(int notidicationId);

		Task DismissNotification(string userId, int notificationId);

		Task DismissAllNotificationsForUser(string userId);

		Task<IEnumerable<NotificationViewModel>> GetTop20ProductRelatedNotification();
	}
}
