using HomeAssistant.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Text.Json;
using ZstdSharp.Unsafe;

namespace HomeAssistant.Controllers
{
	[Authorize(Roles = "StandardUser")]
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class HomeTelemetryController : Controller
	{
		private readonly IHomeTelemetryService _service;

		public HomeTelemetryController(IHomeTelemetryService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<IActionResult> Data()
		{
			string result = await _service.GetData();

			if (string.IsNullOrWhiteSpace(result))
			{
				return StatusCode(501);
			}

			await _service.SaveData(result);

			JsonDocument jsonDocument = JsonDocument.Parse(result);

			return new JsonResult(jsonDocument);
		}
	}
}
