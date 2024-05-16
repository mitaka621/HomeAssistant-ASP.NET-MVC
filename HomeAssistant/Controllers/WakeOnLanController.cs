using HomeAssistant.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAssistant.Controllers
{
	[SkipStatusCodePages]
	[Authorize(Roles = "WakeOnLanAccess")]
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class WakeOnLanController : Controller
	{
		private readonly IWakeOnLanService _wakeOnLanService;
        public WakeOnLanController(IWakeOnLanService service)
        {
            _wakeOnLanService = service;
        }
        public async Task<IActionResult> WakePc(string name)
		{
			
            if (await _wakeOnLanService.WakeOnLan(name))
            {
				return StatusCode(200);
            }

			return StatusCode(409);

		}
	}
}
