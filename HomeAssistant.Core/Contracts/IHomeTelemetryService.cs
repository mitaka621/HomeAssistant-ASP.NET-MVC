namespace HomeAssistant.Core.Contracts
{
	public interface IHomeTelemetryService
	{
		Task<string> GetData();

		Task SaveData(string data);

		Task<int> CreateNotificationIfDataIsAbnormal(string data);
    }
}
