using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAssistant.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UserConfigurationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
