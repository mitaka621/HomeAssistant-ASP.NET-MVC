using HomeAssistant.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HomeAssistant.Controllers
{
	[Authorize(Roles ="StandardUser,NASUser")]
	public class NASController : Controller
	{
		private readonly INASService _service;
        public NASController(INASService service)
        {
			_service=service;
		}

        public async Task<IActionResult> Index(string path="")
		{
			ViewBag.Path = path;

			var data=await _service.GetData(path);


			if (data==null)
            {
				return StatusCode(503);
			}


            return View(data);
		}

		public async Task<IActionResult> DownloadFile(string path="")
		{
			var data = await _service.GetFileString(path);

            if (data.Content.Headers.ContentLength.HasValue)
            {
				Response.Headers.SetCommaSeparatedValues("Content-Length", data.Content.Headers.ContentLength.Value.ToString());
            }

            Stream stream = await data.Content.ReadAsStreamAsync();

            return File(stream, "application/octet-stream", Path.GetFileName(path),true);
        }

		public async Task<IActionResult> GetFilesJson(string path,int skip,int take)
		{
			return Json((await _service.GetData(path, skip, take)).Skip(1));
		}

    }
}
