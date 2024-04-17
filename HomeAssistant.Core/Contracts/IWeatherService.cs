namespace HomeAssistant.Core.Contracts
{
	public interface IWeatherService
	{
		Task<string> GetWeatherJsonString(double lat, double lon);

		Task<string> GetLocationJsonString(double lat, double lon);
	}
}
