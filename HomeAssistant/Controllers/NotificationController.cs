using HomeAssistant.Core.Contracts;
using HomeAssistant.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Security.Claims;

namespace HomeAssistant.Controllers
{
	[SkipStatusCodePages]
	[NoUserLogging]
	[Authorize(Roles = "StandardUser")]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService= notificationService;
        }

        [HttpGet]
        public async Task< IActionResult> GetNotificationsForUserJson(int skip, int take)
        {
            return Json(await _notificationService.GetNotificationsForUser(GetUserId(),take,skip));
        }

		[HttpGet]
		public async Task<IActionResult> DismissAllNotificationsForUser()
		{
            await _notificationService.DismissAllNotificationsForUser(GetUserId());
			return Ok();
		}

		private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
