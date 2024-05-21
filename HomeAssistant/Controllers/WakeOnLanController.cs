using HomeAssistant.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.Operations;

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
			try
			{
				if (await _wakeOnLanService.WakeOnLan(name))
				{
					return StatusCode(200);
				}

				return StatusCode(409);
			}
			catch (Exception)
			{
				return StatusCode(500);
			}
			

		}

		public async Task<IActionResult> PingPc(string name)
		{
			try
			{
				var pcs = _wakeOnLanService.GetAllAvailableWakeOnLanPcs();

				var pc = pcs.FirstOrDefault(x => x.PcName == name);

				if (pc == null || (!pc.IsNAS && !pc.IsHostPc))
				{
					return BadRequest();
				}

				if (await _wakeOnLanService.PingHost(pc.PcIp))
				{
					return StatusCode(200);
				}

				return StatusCode(409);
			}
			catch (Exception)
			{
				return StatusCode(500);
			}
			

		}

		public IActionResult GetAvailiblePCs()
		{
			try
			{
				return Json(_wakeOnLanService
				.GetAllAvailableWakeOnLanPcs()
				.Select(x => new
				{
					x.IsHostPc,
					x.IsNAS,
					x.PcIp,
					x.PcName,
				}));
			}
			catch (Exception)
			{
				return StatusCode(500);
			}
			
		}
	}
}
