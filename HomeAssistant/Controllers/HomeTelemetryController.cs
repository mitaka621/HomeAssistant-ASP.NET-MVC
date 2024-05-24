using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Enums;
using HomeAssistant.Core.Services;
using HomeAssistant.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net.Sockets;
using System.Text.Json;
using ZstdSharp.Unsafe;

namespace HomeAssistant.Controllers
{
	[SkipStatusCodePages]
	[Authorize(Roles = "StandardUser")]
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class HomeTelemetryController : Controller
	{
		private readonly IHomeTelemetryService _service;
		private readonly IHubContext<NotificationsHub> _notificationHubContext;
		private readonly INotificationService _notificationService;


		public HomeTelemetryController(IHomeTelemetryService service, IHubContext<NotificationsHub> notificationHubContext, INotificationService notificationService)
		{
			_service = service;
			_notificationHubContext = notificationHubContext;
			_notificationService = notificationService;
		}

		[Route("~/HomeTelemetry/Index")]
		[HttpGet]
		public async Task< IActionResult> Index(DateTime? startDate, DateTime? endDate, DataRangeEnum dataRange = DataRangeEnum.Hour,string? type="Radiation",BarsPerPage count= BarsPerPage.Bars_10)
		{
			var models = await _service.GetDataRange(dataRange, startDate, endDate, count);

			ViewBag.DateRange = dataRange;
			ViewBag.Type = type;
			ViewBag.BarsCount = count;
			if (models.Count==0)
            {
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;

                return View(models);
            }
         
            ViewBag.StartDate = models.FirstOrDefault().Key;
            ViewBag.EndDate = models.LastOrDefault().Key;

            return View(models);
		}

		[HttpGet]
		public async Task<IActionResult> Data()
		{
			string result = await _service.GetLiveData();

			if (string.IsNullOrWhiteSpace(result))
			{
				return StatusCode(501);
			}

            JsonDocument jsonDocument = JsonDocument.Parse(result);

			return new JsonResult(jsonDocument);
		}
	}
}
