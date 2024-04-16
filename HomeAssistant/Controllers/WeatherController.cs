using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models;
using HomeAssistant.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace HomeAssistant.Controllers
{
	[Authorize(Roles ="StandardUser")]
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class WeatherController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly IWeatherService _weatherService;

        public WeatherController(IUserService userService, IWeatherService weatherService)
        {
			_userService=userService;
			_weatherService=weatherService;
		}

		[HttpGet]
		public async Task<IActionResult> GetWeather()
		{
			var user = await _userService.GetUserByIdAsync(GetUserId());
			string result = await _weatherService.GetWeatherJsonString(user.Latitude, user.Longitude);

			JsonDocument jsonDocument = JsonDocument.Parse(result);
			
			return new JsonResult(jsonDocument);
		}

		[HttpGet]
		public async Task<IActionResult> UserLocation()
		{
			var user = await _userService.GetUserByIdAsync(GetUserId());
			string result = await _weatherService.GetLocationJsonString(user.Latitude, user.Longitude);

			JsonDocument jsonDocument = JsonDocument.Parse(result);

			return new JsonResult(jsonDocument);
		}

		[HttpPost]
		[IgnoreAntiforgeryToken]
		public async Task< IActionResult> UserLocation([FromBody]UserLocationDto data)
		{
			if (await _userService.AddUserLocation(GetUserId(), data.Latitude, data.Longitude))
				return StatusCode(200);
			else
				return BadRequest();
		}

		private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
