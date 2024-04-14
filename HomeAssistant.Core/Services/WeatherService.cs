using HomeAssistant.Core.Contracts;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Services
{
	public class WeatherService : IWeatherService
	{
		private readonly string apiKey;
		private readonly HttpClient httpClient;
		private readonly UserManager<HomeAssistantUser> userManager;

		public WeatherService(HttpClient _httpClient,IConfiguration configuration,UserManager<HomeAssistantUser> _userManager)
		{
			apiKey = configuration.GetSection("ExternalServiceApiKeys")["weatherApi"];
			httpClient = _httpClient;
			userManager= _userManager;

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
