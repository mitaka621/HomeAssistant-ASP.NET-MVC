using Amazon.Runtime.Internal.Transform;
using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Enums;
using HomeAssistant.Core.Models.HomeTelemetry;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
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
            if (configuration.GetSection("HomeTelemetryServerIp").Value == null)
            {
                serverIp = "0.0.0.0";
            }
            else
            {
                serverIp = configuration.GetSection("HomeTelemetryServerIp").Value;
            }

            httpClient = _httpClient;
            httpClient.BaseAddress = new Uri("http://" + serverIp);

            _dbcontext = dbcontext;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<string> GetLiveData()
        {
            try
            {
                return await httpClient
                .GetStringAsync("/data");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to get telemetry");

                return "";
            }

        }

        public async Task SaveData(string data)
        {
            var obj = JsonSerializer.Deserialize<HomeTelemetryViewModel>(data);

            if (obj == null)
            {
                throw new ArgumentNullException();
            }

            var prevRecord = _dbcontext.homeTelemetries.OrderByDescending(x => x.DateTime).FirstOrDefault();

            if (prevRecord != null &&
                prevRecord.CPM == obj.CPM &&
                prevRecord.Temperature == obj.Temperature &&
                prevRecord.Humidity == obj.Humidity &&
                prevRecord.Radiation == obj.Radiation)
            {
                return;
            }

            obj.DateTime = DateTime.Now;

            _dbcontext.homeTelemetries.Add(new HomeTelemetry()
            {
                DateTime = obj.DateTime,
                CPM = obj.CPM,
                Temperature = obj.Temperature,
                Humidity = obj.Humidity,
                Radiation = obj.Radiation
            });

            await _dbcontext.SaveChangesAsync();
        }

        public async Task<int> CreateNotificationIfDataIsAbnormal(string data)
        {
            var obj = JsonSerializer.Deserialize<HomeTelemetry>(data);

            if (obj == null)
            {
                throw new ArgumentNullException();
            }

            if (obj.Radiation < 1)
            {
                return -1;
            }

            var notificationId = await _notificationService
                .CreateNotificationForAllUsers("Abnormal radiation levels detected", $"At {DateTime.Now} radiation levels of {obj.Radiation} µSv were detected!", "/System", null);

            return notificationId;
        }

        public async Task<Dictionary<DateTime, HomeTelemetryViewModel>> GetDataRange(
            DataRangeEnum dataRange,
            DateTime? startDate,
            DateTime? endDate,
            BarsPerPage numBars)
        {
            int numBarsInt;
            if (numBars==BarsPerPage.Default)
            {
				numBarsInt = 100;
			}
            else
            {
				numBarsInt = (int)numBars * 10;
			}

            var data = await _dbcontext
                .homeTelemetries
                .AsNoTracking()
                .Where(x => x.DateTime > (startDate ?? DateTime.MinValue) && x.DateTime < (endDate ?? DateTime.Now))
                .OrderBy(x=>x.DateTime)
                .Select(x => new HomeTelemetryViewModel()
                {
                    CPM = x.CPM,
                    DateTime = x.DateTime,
                    Temperature = x.Temperature,
                    Humidity = x.Humidity,
                    Radiation = x.Radiation,
                    Id = x.Id
                })
                .ToListAsync();

            if (data.Count == 0)
            {
                return new Dictionary<DateTime, HomeTelemetryViewModel>();
            }

            TimeSpan interval;

            switch (dataRange)
            {
                case DataRangeEnum.Hour:
                    interval = new TimeSpan(1, 0, 0);
                    break;
                case DataRangeEnum.Day:
                    interval = new TimeSpan(1, 0, 0, 0);
                    break;
                case DataRangeEnum.Month:
                    interval = new TimeSpan(30, 0, 0, 0);
                    break;
                default:
                    if (startDate == null)
                    {
                        return data.TakeLast(numBarsInt).Select(x => new KeyValuePair<DateTime, HomeTelemetryViewModel>(x.DateTime, x))
                        .ToDictionary(x => x.Key, x => x.Value);
                    }
                    return data.Take(numBarsInt).Select(x => new KeyValuePair<DateTime, HomeTelemetryViewModel>(x.DateTime, x))
                        .ToDictionary(x => x.Key, x => x.Value);
            }

            DateTime targetDateTime = data[0].DateTime + interval;

            double avgHumidity = 0, avgRadiation = 0, avgCPM = 0, avgTempreture = 0;
            int count = 0;
            double avgDateTime =  0;

           
			Dictionary <DateTime, HomeTelemetryViewModel> outputData = new();

            foreach (var record in data)
            {
                if (record.DateTime <= targetDateTime)
                {
                    avgHumidity += record.Humidity;
                    avgRadiation += record.Radiation;
                    avgCPM += record.CPM;
                    avgTempreture += record.Temperature;
                    avgDateTime +=new TimeSpan( record.DateTime.Ticks).TotalSeconds;

					count++;

                    continue;
                }

                if (count != 0)
                {
                    double seconds = avgDateTime / count;

					outputData.Add(new DateTime().AddSeconds(seconds), new HomeTelemetryViewModel()
                    {
                        CPM = avgCPM / count,
                        Humidity = avgHumidity / count,
                        Radiation = avgRadiation / count,
                        Temperature = avgTempreture / count,
                    });
                }


                avgHumidity = 0;
                avgRadiation = 0;
                avgCPM = 0;
                avgTempreture = 0;
                avgDateTime = 0;

				count = 0;

                targetDateTime = record.DateTime + interval;
            }

            if (startDate==null)
            {
                return outputData.TakeLast(numBarsInt).ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            return outputData.Take(numBarsInt).ToDictionary(pair => pair.Key, pair => pair.Value);

        }
    }
}
