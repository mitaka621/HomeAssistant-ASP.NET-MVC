using HomeAssistant.Core.Contracts;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HomeAssistant.Core.Services
{
	public class HomeTelemetryService : IHomeTelemetryService
	{
		private readonly string serverIp;
		private readonly HttpClient httpClient;
		private readonly HomeAssistantDbContext _dbcontext;
		private readonly ILogger _logger;
		private readonly INotificationService _notificationService;

		public HomeTelemetryService(IConfiguration configuration, HttpClient _httpClient, HomeAssistantDbContext dbcontext, ILogger<IHomeTelemetryService> logger, INotificationService notificationService)
        {
            if (configuration.GetSection("HomeTelemetryServerIp").Value==null)
            {
				serverIp ="0.0.0.0";
			}
			else
			{
				serverIp = configuration.GetSection("HomeTelemetryServerIp").Value;
			}

			httpClient= _httpClient;
			httpClient.BaseAddress = new Uri("http://"+serverIp);

			_dbcontext=dbcontext;
			_notificationService=notificationService;
			_logger = logger;
		}

		public async Task<string> GetData()
		{
			try
			{
				return await httpClient
				.GetStringAsync("/data");
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex,"Unable to get telemetry");

				return "";
			}
			
		}

		public async Task SaveData(string data)
		{
			var obj=JsonSerializer.Deserialize<HomeTelemetry>(data);

            if (obj==null)
            {
				throw new ArgumentNullException();
            }

			var prevRecord=_dbcontext.homeTelemetries.OrderByDescending(x=>x.DateTime).FirstOrDefault();

            if (prevRecord!=null&&
				prevRecord.CPM==obj.CPM&&
				prevRecord.Temperature==obj.Temperature&&
				prevRecord.Humidity==obj.Humidity&&
				prevRecord.Radiation==obj.Radiation)
            {
				return;
            }

            obj.DateTime= DateTime.Now;

			_dbcontext.homeTelemetries.Add(obj);

			await _dbcontext.SaveChangesAsync();
		}

		public async Task<int> CreateNotificationIfDataIsAbnormal(string data)
		{
			var obj = JsonSerializer.Deserialize<HomeTelemetry>(data);

			if (obj == null)
			{
				throw new ArgumentNullException();
			}

            if (obj.Radiation<1)
            {
				return -1;
            }

			var notificationId=await _notificationService
				.CreateNotificationForAllUsers("Abnormal radiation levels detected", $"At {DateTime.Now} radiation levels of {obj.Radiation} µSv were detected!", "/System", null);

			return notificationId;
        }
	}
}
