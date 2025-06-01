using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Notification;
using HomeAssistant.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HomeAssistant.BackgroundServiceJobs
{
    public class CheckTimerExpirationService : BackgroundService
    {
        private readonly IHubContext<NotificationsHub> _notificationHubContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CheckTimerExpirationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10);

        public CheckTimerExpirationService(IHubContext<NotificationsHub> hubContext, IServiceProvider serviceProvider, ILogger<CheckTimerExpirationService> logger, IConfiguration configuration)
        {
            _notificationHubContext = hubContext;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                        var timerManager = scope.ServiceProvider.GetRequiredService<IRecipeService>();
                        var usersWithExpiredTimers = await timerManager.GetUsersWithExpiredTimers();

                        foreach (var tuple in usersWithExpiredTimers)
                        {
                            var notificationId = await _notificationService.CreateNotificationForSpecificUser(
                            $"Timer for {tuple.Item3} expired",
                            "You are ready to porceed to the next step!",
                            $"/Recipe/RecipeStep?recipeId={tuple.Item2}",
                            tuple.Item1,
                            null);

                            await _notificationService.PushNotificationForUser(tuple.Item1,
                                $"Timer expired",
                                $"Timer for {tuple.Item3} expired. You are ready to porceed to the next step!",
                                $"{_configuration.GetSection("Configuration")["PublicURL"]}/Recipe/RecipeStep?recipeId={tuple.Item2}",
                                $"{_configuration.GetSection("Configuration")["PublicURL"]}/svg/cooking-pot.svg"
                            );

                            _ = _notificationHubContext.Clients
                            .User(tuple.Item1)
                            .SendAsync("PushNotfication", new NotificationViewModel()
                            {
                                Id = notificationId,
                                CreatedOn = DateTime.Now,
                                Description = "You are ready to porceed to the next step!",
                                Invoker = new NotificationUserViewModel()
                                {
                                    Id = tuple.Item1,
                                    FirstName = "System"
                                },
                                Source = $"/Recipe",
                                Title = $"Timer for {tuple.Item3} expired",
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Background check timer job error occured");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }

}
