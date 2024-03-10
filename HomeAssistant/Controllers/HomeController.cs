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

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var roles = (await _userService.GetUserByIdAsync(GetUserId())).UserRoles;
                if (!roles.Contains("StandardUser"))
                {
                    return RedirectToAction(nameof(WaitingApproval));
                }
            }
            catch (ArgumentNullException) 
            {
				return RedirectToAction(nameof(WaitingApproval));
			}
            return View(await _userService.GetUserByIdAsync(GetUserId()));
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

		private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
