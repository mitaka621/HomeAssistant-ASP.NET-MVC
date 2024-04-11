using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Notification;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.Core.Services
{
	public class NotificationService : INotificationService
	{
		private readonly HomeAssistantDbContext _dbcontext;
		private readonly IUserService _userService;
		private readonly IimageService _imageService;

		public NotificationService(HomeAssistantDbContext dbcontext, IUserService userService, IimageService imageService)
		{
			_dbcontext = dbcontext;
			_userService = userService;
			_imageService = imageService;
		}

		public async Task<int> CreateNotificationForAllUsers(string title, string description, string invokerURL, string? invokerId)
		{
			var notificationModel = new Notification()
			{
				Title = title,
				Description = description,
				InvokedBy = invokerId,
				InvokerURL = invokerURL,
				CreatedOn = DateTime.Now,
				NotificationsUsers = (await _userService.GetAllApprovedNotDeletedUsersAsync())
				.Select(x => new NotificationUser()
				{
					UserId = x
				}).ToList()
			};

			_dbcontext.Notifications.Add(notificationModel);
			await _dbcontext.SaveChangesAsync();

			return notificationModel.Id;

		}

		public async Task<int> CreateNotificationForSpecificUsers(string title, string description, string invokerURL, string userId, string? invokerId)
		{

			if ((await _userService.GetAllApprovedNotDeletedUsersAsync()).FirstOrDefault(x => x == userId) == null)
			{
				throw new ArgumentNullException();
			}

			var notificationModel = new Notification()
			{
				Title = title,
				Description = description,
				InvokedBy = invokerId,
				InvokerURL = invokerURL,
				CreatedOn = DateTime.Now,
				NotificationsUsers = new List<NotificationUser>()
					{
						new NotificationUser()
						{
							UserId = userId
						}
					}
			};

			_dbcontext.Notifications.Add(notificationModel);
			await _dbcontext.SaveChangesAsync();

			return notificationModel.Id;

		}

		public async Task<IEnumerable<NotificationViewModel>> GetNotificationsForUser(string userId)
		{
			var notifications = await _dbcontext.NotificationsUsers
				.AsNoTracking()
				.Where(x => x.UserId == userId)
				.Select(x => new NotificationViewModel()
				{
					Id = x.NotificationId,
					Title = x.Notification.Title,
					Description = x.Notification.Description,
					CreatedOn = x.Notification.CreatedOn,
					Source = x.Notification.InvokerURL,
					Invoker = new NotificationUserViewModel()
					{
						Id = x.Notification.User == null ? null : x.Notification.User.Id,
						FirstName = x.Notification.User == null ? null : x.Notification.User.FirstName,
					}
				})
				.OrderByDescending(x => x.CreatedOn)
				.ToListAsync();

			for (int i = 0; i < notifications.Count; i++)
			{
				notifications[i].Source = notifications[i].Source.Split("/")[1];

                if (notifications[i].Invoker.Id==null)
                {
					continue;
                }
                notifications[i].Invoker.Photo = await _imageService.GetPFP(notifications[i].Invoker.Id);
			}

			return notifications;
		}

		public async Task<NotificationViewModel> GetNotification(int notidicationId)
		{
			var notification = await _dbcontext.Notifications
				.AsNoTracking()
				.Include(x => x.User)
				.FirstOrDefaultAsync(x => x.Id == notidicationId);

			if (notification == null)
			{
				throw new ArgumentNullException(nameof(notification));
			}

			var model = new NotificationViewModel()
			{
				Id = notidicationId,
				Title = notification.Title,
				Description = notification.Description,
				CreatedOn = notification.CreatedOn,
				Source = notification.InvokerURL.Split("/")[1],
				Invoker = new NotificationUserViewModel()
				{
					Id = notification.User == null ? null : notification.User.Id,
					FirstName = notification.User == null ? null : notification.User.FirstName,
					Photo= notification.User == null ? new byte[0] : await _imageService.GetPFP( notification.User.Id)
				}
			};

			return model;
		}
	}
}
