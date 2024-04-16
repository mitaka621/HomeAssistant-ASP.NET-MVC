using HomeAssistant.Core.Contracts;
using Microsoft.Extensions.Configuration;

namespace HomeAssistant.Core.Services
{
	public class WeatherService : IWeatherService
	{
		private readonly string apiKey;
		private readonly HttpClient httpClient;

		public WeatherService(HttpClient _httpClient,IConfiguration configuration)
		{
			apiKey = configuration.GetSection("ExternalServiceApiKeys")["weatherApi"];
			httpClient = _httpClient;

			httpClient.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
		}

		public async Task<string> GetWeatherJsonString(double lat, double lon)
		{
			return await httpClient
				.GetStringAsync($"weather?lat={lat}&lon={lon}&appid={apiKey}&units=metric");
		}

		public async Task<string> GetLocationJsonString(double lat, double lon)
		{
			httpClient.BaseAddress = new Uri("https://api.openweathermap.org/geo/1.0/");
			return await httpClient
				.GetStringAsync($"reverse?lat={lat}&lon={lon}&limit=1&appid={apiKey}");
		}
	}
}
