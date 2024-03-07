using HomeAssistant.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAssistant.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UserConfigurationController : Controller
    {
        IUserService userService;
        public UserConfigurationController(IUserService _userService)
        {
            userService= _userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            return View(await userService.GetAllNotApprovedUsers());
        }
    }
}
