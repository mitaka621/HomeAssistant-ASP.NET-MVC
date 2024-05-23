using HomeAssistant.Core.Enums;
using HomeAssistant.Core.Models.HomeTelemetry;

namespace HomeAssistant.Core.Contracts
{
	public interface IHomeTelemetryService
	{
		Task<string> GetLiveData();

		Task<Dictionary<DateTime, HomeTelemetryViewModel>> GetDataRange(DataRangeEnum dataRange,DateTime? startDate, DateTime? endDate);

		Task SaveData(string data);

		Task<int> CreateNotificationIfDataIsAbnormal(string data);
    }
}
