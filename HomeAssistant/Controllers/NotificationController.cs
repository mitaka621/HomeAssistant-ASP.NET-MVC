using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Notification;
using HomeAssistant.Filters;
using HomeAssistant.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly HomeAssistantDbContext _dbcontext;

        public NotificationController(INotificationService notificationService, HomeAssistantDbContext dbcontext)
        {
            _notificationService= notificationService;
            _dbcontext= dbcontext;
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

		[HttpPost]
		[IgnoreAntiforgeryToken]
		public async Task<IActionResult> RemoveSubscription(string deviceType)
		{
            var subscription=await _dbcontext.UserSubscriptions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.DeviceType == deviceType && x.UserId == GetUserId());

            if (subscription==null)
            {
                return BadRequest();
            }

            _dbcontext.UserSubscriptions.Remove(subscription);
            await _dbcontext.SaveChangesAsync();

			return Ok();
		}

		[HttpGet]
        public async Task<IActionResult> CheckSubscription(string deviceType)
        {
            if (await _dbcontext.UserSubscriptions
                .AsNoTracking()
                .AnyAsync(x=>x.DeviceType==deviceType&&x.UserId==GetUserId())
                )
            {
                return Ok();
            }

            return NotFound();
        }

		private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
