﻿using System.Text.Json;
using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Notification;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebPush;

namespace HomeAssistant.Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HomeAssistantDbContext _dbcontext;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public NotificationService(HomeAssistantDbContext dbcontext, IUserService userService, IImageService imageService, IConfiguration configuration, ILogger<INotificationService> logger)
        {
            _dbcontext = dbcontext;
            _userService = userService;
            _imageService = imageService;
            _configuration = configuration;
            _logger = logger;
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
                    UserId = x.Id
                }).ToList()
            };

            _dbcontext.Notifications.Add(notificationModel);
            await _dbcontext.SaveChangesAsync();

            return notificationModel.Id;

        }

        public async Task<int> CreateNotificationForSpecificUser(string title, string description, string invokerURL, string userId, string? invokerId)
        {

            if ((await _userService.GetAllApprovedNotDeletedUsersAsync()).FirstOrDefault(x => x.Id == userId) == null)
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

        public async Task<NotificationsWithPfpModel> GetNotificationsForUser(string userId, int take = 10, int skip = 0)
        {
            var notifications = await _dbcontext.NotificationsUsers
                .AsNoTracking()
                .Where(x => x.UserId == userId && !x.IsDismissed)
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
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            notifications
                .ForEach(x =>
                {
                    x.Source = x.Source.Split("/")[1];
                });

            return new NotificationsWithPfpModel()
            {
                NotificationsContent = notifications,
            };
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
                }
            };

            return model;
        }


        public async Task<int> CreateNotificationForAllUsersExceptOne(
            string title,
            string description,
            string exceptUserId,
            string invokerURL,
            string? invokerId)
        {

            var notificationModel = new Notification()
            {
                Title = title,
                Description = description,
                InvokedBy = invokerId,
                InvokerURL = invokerURL,
                CreatedOn = DateTime.Now,
                NotificationsUsers = (await _userService.GetAllApprovedNotDeletedUsersIds())
                .Where(x => x != exceptUserId)
                .Select(x => new NotificationUser()
                {
                    UserId = x
                }).ToList()
            };

            _dbcontext.Notifications.Add(notificationModel);
            await _dbcontext.SaveChangesAsync();

            return notificationModel.Id;
        }

        public async Task DismissNotification(string userId, int notificationId)
        {
            var notification = await _dbcontext.NotificationsUsers
                .FirstOrDefaultAsync(x => x.UserId == userId && x.NotificationId == notificationId);

            if (notification == null)
            {
                throw new ArgumentNullException();
            }

            notification.IsDismissed = true;
            await _dbcontext.SaveChangesAsync();
        }

        public async Task<IEnumerable<NotificationViewModel>> GetTop20ProductRelatedNotification()
        {
            var notifications = await _dbcontext.Notifications
                .AsNoTracking()
                .Include(x => x.User)
                .OrderByDescending(x => x.CreatedOn)
                .Where(x => x.InvokerURL.Contains("/ShoppingList/FinishShoppingList") ||
                x.InvokerURL.Contains("/Fridge") ||
                x.InvokerURL.Contains("/Recipe/FinishRecipe"))
                .Take(20)
                .ToListAsync();

            List<NotificationViewModel> model = new List<NotificationViewModel>();

            var images = await _imageService.GetPfpRange(notifications
                .Where(x => x != null)
                .Select(x => x.InvokedBy ?? "")
                .Distinct().ToArray());

            foreach (var item in notifications)
            {
                if (item.InvokerURL.Contains("/Fridge"))
                {

                    model.Add(new NotificationViewModel()
                    {
                        CreatedOn = item.CreatedOn,
                        Title = item.Title,
                        Invoker = new NotificationUserViewModel()
                        {
                            FirstName = item.User == null ? "" : item.User.FirstName,

                        }
                    });

                    continue;
                }

                foreach (var product in item.Description.Split("\r\n").Where(x => x.Length > 1).Skip(1))
                {
                    var modelToAdd = new NotificationViewModel()
                    {
                        CreatedOn = item.CreatedOn,
                        Invoker = new NotificationUserViewModel()
                        {
                            FirstName = item.User == null ? "" : item.User.FirstName,
                        }
                    };

                    if (item.InvokerURL.Contains("ShoppingList"))
                    {
                        modelToAdd.Title = $"{product.Split("(")[1]} {product.Split("(")[0]} added to fridge";
                    }
                    else
                    {
                        modelToAdd.Title = $"{product.Split("(")[1]} {product.Split("(")[0]} removed from fridge";
                    }

                    model.Add(modelToAdd);
                }
            }

            return model;
        }

        public async Task DismissAllNotificationsForUser(string userId)
        {
            var notifications = await _dbcontext.NotificationsUsers.Where(x => x.UserId == userId).ToListAsync();

            notifications.ForEach(x => x.IsDismissed = true);

            await _dbcontext.SaveChangesAsync();
        }

        public async Task SubscribeUserForPush(PushNotificationRegistrationModel model, string userId)
        {
            var user = await _dbcontext.Users
                .Include(x => x.UserSubscriptions)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var subscriptionModel = user.UserSubscriptions.FirstOrDefault(x => x.DeviceType == model.DeviceType);

            if (subscriptionModel == null)
            {
                subscriptionModel = new();

                subscriptionModel.PushNotificationAuth = model.Keys.Auth;
                subscriptionModel.DeviceType = model.DeviceType;
                subscriptionModel.P256dh = model.Keys.P256dh;
                subscriptionModel.PushNotificationEndpoint = model.Endpoint;
                subscriptionModel.UserId = userId;

                _dbcontext.Add(subscriptionModel);

                await _dbcontext.SaveChangesAsync();
            }
            else
            {
                subscriptionModel.PushNotificationAuth = model.Keys.Auth;
                subscriptionModel.DeviceType = model.DeviceType;
                subscriptionModel.P256dh = model.Keys.P256dh;
                subscriptionModel.PushNotificationEndpoint = model.Endpoint;

                await _dbcontext.SaveChangesAsync();
            }
        }

        public async Task<bool> PushNotificationForUser(string userId, string title, string body, string url, string? iconUrl)
        {
            var userSubscriptions = await _dbcontext.UserSubscriptions.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();

            if (userSubscriptions.Count == 0)
            {
                return false;
            }

            string? vapidPublicKey = _configuration.GetSection("VAPID")["PublicKey"];
            string? vapidPrivateKey = _configuration.GetSection("VAPID")["PrivateKey"];
            string? vapidEmail = _configuration.GetSection("VAPID")["Mail"];

            if (string.IsNullOrEmpty(vapidPublicKey) || string.IsNullOrEmpty(vapidPrivateKey) || string.IsNullOrEmpty(vapidEmail))
            {
                return false;
            }

            var vapidDetails = new VapidDetails(vapidEmail, vapidPublicKey, vapidPrivateKey);

            var payload = new
            {
                title = title,
                body = body,
                url = url,
                badge = $"{_configuration.GetSection("Configuration")["PublicURL"]}/svg/badge.png",
                icon = iconUrl ?? $"{_configuration.GetSection("Configuration")["PublicURL"]}/favicon.ico",
            };
            var payloadJson = JsonSerializer.Serialize(payload);

            List<Task> pushTasks = new();

            foreach (var subscription in userSubscriptions)
            {

                var pushSubscription = new PushSubscription(subscription.PushNotificationEndpoint, subscription.P256dh, subscription.PushNotificationAuth);

                var webPushClient = new WebPushClient();
                try
                {
                    pushTasks.Add(webPushClient.SendNotificationAsync(pushSubscription, payloadJson, vapidDetails));
                }
                catch (WebPushException exception)
                {
                    _logger.LogError("Failed to send push notification. Http STATUS code" + exception.StatusCode);
                }
            }

            try
            {
                await Task.WhenAll(pushTasks);
            }
            catch (WebPushException exception)
            {
                await RemoveSubscription(exception.PushSubscription.Endpoint, exception.PushSubscription.Auth);

                _logger.LogWarning("Removing subscription");
            }

            return true;
        }

        public async Task<bool> PushNotificationForAllUsers(string title, string body, string url, string? iconUrl)
        {
            var userSubscriptions = await _dbcontext.UserSubscriptions
                .AsNoTracking()
                .ToListAsync();

            if (userSubscriptions.Count == 0)
            {
                return false;
            }

            string? vapidPublicKey = _configuration.GetSection("VAPID")["PublicKey"];
            string? vapidPrivateKey = _configuration.GetSection("VAPID")["PrivateKey"];
            string? vapidEmail = _configuration.GetSection("VAPID")["Mail"];

            if (string.IsNullOrEmpty(vapidPublicKey) || string.IsNullOrEmpty(vapidPrivateKey) || string.IsNullOrEmpty(vapidEmail))
            {
                return false;
            }

            var vapidDetails = new VapidDetails(vapidEmail, vapidPublicKey, vapidPrivateKey);

            var payload = new
            {
                title = title,
                body = body,
                url = url,
                badge = $"{_configuration.GetSection("Configuration")["PublicURL"]}/svg/badge.png",
                icon = iconUrl ?? $"{_configuration.GetSection("Configuration")["PublicURL"]}/favicon.ico",
            };
            var payloadJson = JsonSerializer.Serialize(payload);

            List<Task> pushTasks = new();

            foreach (var subscription in userSubscriptions)
            {

                var pushSubscription = new PushSubscription(subscription.PushNotificationEndpoint, subscription.P256dh, subscription.PushNotificationAuth);

                var webPushClient = new WebPushClient();
                try
                {
                    pushTasks.Add(webPushClient.SendNotificationAsync(pushSubscription, payloadJson, vapidDetails));
                }
                catch (WebPushException exception)
                {
                    _logger.LogError("Failed to send push notification. Http STATUS code" + exception.StatusCode);
                }
            }

            try
            {
                await Task.WhenAll(pushTasks);
            }
            catch (WebPushException exception)
            {
                await RemoveSubscription(exception.PushSubscription.Endpoint, exception.PushSubscription.Auth);

                _logger.LogWarning("Removing subscription");
            }


            return true;
        }

        public async Task<bool> PushNotificationForAllUsersExcept(string userIdToExclude, string title, string body, string url, string? iconUrl)
        {
            var userSubscriptions = await _dbcontext.UserSubscriptions
                .AsNoTracking()
                .Where(x => x.UserId != userIdToExclude)
                .ToListAsync();

            if (userSubscriptions.Count == 0)
            {
                return false;
            }

            string? vapidPublicKey = _configuration.GetSection("VAPID")["PublicKey"];
            string? vapidPrivateKey = _configuration.GetSection("VAPID")["PrivateKey"];
            string? vapidEmail = _configuration.GetSection("VAPID")["Mail"];

            if (string.IsNullOrEmpty(vapidPublicKey) || string.IsNullOrEmpty(vapidPrivateKey) || string.IsNullOrEmpty(vapidEmail))
            {
                return false;
            }

            var vapidDetails = new VapidDetails(vapidEmail, vapidPublicKey, vapidPrivateKey);

            var payload = new
            {
                title = title,
                body = body,
                url = url,
                badge = $"{_configuration.GetSection("Configuration")["PublicURL"]}/svg/badge.png",
                icon = iconUrl ?? $"{_configuration.GetSection("Configuration")["PublicURL"]}/favicon.ico",
            };
            var payloadJson = JsonSerializer.Serialize(payload);

            List<Task> pushTasks = new();

            foreach (var subscription in userSubscriptions)
            {

                var pushSubscription = new PushSubscription(subscription.PushNotificationEndpoint, subscription.P256dh, subscription.PushNotificationAuth);

                var webPushClient = new WebPushClient();
                try
                {
                    pushTasks.Add(webPushClient.SendNotificationAsync(pushSubscription, payloadJson, vapidDetails));
                }
                catch (WebPushException exception)
                {
                    _logger.LogError("Failed to send push notification. Http STATUS code" + exception.StatusCode);
                }
            }

            try
            {
                await Task.WhenAll(pushTasks);
            }
            catch (WebPushException exception)
            {
                await RemoveSubscription(exception.PushSubscription.Endpoint, exception.PushSubscription.Auth);

                _logger.LogWarning("Removing subscription");
            }

            return true;
        }

        public async Task RemoveSubscription(string endPoint, string auth)
        {
            var subscription = await _dbcontext.UserSubscriptions.FirstOrDefaultAsync(x => x.PushNotificationEndpoint == endPoint && x.PushNotificationAuth == auth);

            if (subscription == null)
            {
                return;
            }

            _dbcontext.UserSubscriptions.Remove(subscription);

            await _dbcontext.SaveChangesAsync();
        }
    }
}
