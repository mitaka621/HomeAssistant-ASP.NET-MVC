
using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Services;
using HomeAssistant.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HomeAssistant.BackgroundServiceJobs
{
	public class WriteHomeTelemetryDataToDb : BackgroundService
	{
		private readonly IHubContext<NotificationsHub> _notificationHubContext;
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<WriteHomeTelemetryDataToDb> _logger;

		private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

		public WriteHomeTelemetryDataToDb(IHubContext<NotificationsHub> hubContext, IServiceProvider serviceProvider, ILogger<WriteHomeTelemetryDataToDb> logger)
		{
			_notificationHubContext = hubContext;
			_serviceProvider = serviceProvider;
			_logger = logger;
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
						var _homeTelemetryService = scope.ServiceProvider.GetRequiredService<IHomeTelemetryService>();

						string data = await _homeTelemetryService.GetData();

						if (string.IsNullOrWhiteSpace(data))
						{
							throw new ArgumentException("Recieved Invalid data from home telemetry server. Data:" + data);
						}

						await _homeTelemetryService.SaveData(data);

						var notificationId = await _homeTelemetryService.CreateNotificationIfDataIsAbnormal(data);


						if (notificationId != -1)
						{
							await _notificationHubContext.Clients.All.SendAsync("PushNotfication", await _notificationService.GetNotification(notificationId));
						}
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Background WriteHomeTelemetrytoDb job error occured");
				}

				await Task.Delay(_checkInterval, stoppingToken);
			}
		}
	}
}
