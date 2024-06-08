using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Notification;
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

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Subscribe([FromBody] PushNotificationRegistrationModel model)
        {
            try
            {
               await _notificationService.SubscribeUserForPush(model, GetUserId());
            }
            catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }

		private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
