using HomeAssistant.Core.Contracts;
using HomeAssistant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace HomeAssistant.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        public HomeController(IUserService userService, INotificationService notificationService)
        {
            _userService = userService;
			_notificationService = notificationService;

		}

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var roles = (await _userService.GetUserByIdAsync(GetUserId())).UserRoles;

				if (roles.Contains("Admin")&&!roles.Contains("StandardUser"))
				{
					return LocalRedirect("/UserConfiguration");
				}

				if (!roles.Contains("StandardUser"))
                {
                    return RedirectToAction(nameof(WaitingApproval));
                }
				
			}
            catch (ArgumentNullException) 
            {
				return RedirectToAction(nameof(WaitingApproval));
			}
            var model = await _userService.GetUserByIdAsync(GetUserId());

            model.Notifications=await _notificationService.GetNotificationsForUser(GetUserId());

			return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> WaitingApproval()
        {
			try
			{
				var roles = (await _userService.GetUserByIdAsync(GetUserId())).UserRoles;
				if (roles.Contains("StandardUser"))
				{
					return RedirectToAction(nameof(Index));
				}
			}
			catch (ArgumentNullException)
			{
				return RedirectToAction(nameof(Index));
			}
			return View();
		}

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public IActionResult StatusCode(int id)
        {
            if (id == 404|| id == 500)
            {
                return View("Status" + id);
            }

            return View(id);
        }

		private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
