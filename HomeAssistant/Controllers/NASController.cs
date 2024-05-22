using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;

namespace HomeAssistant.Controllers
{
	[Authorize(Roles ="NASUser")]
	public class NASController : Controller
	{
		private readonly INASHostService _service;
        public NASController(INASHostService service)
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

			ViewBag.currentHostIP = NASHostService.currentHostIp;
            return View(data);
		}

		public async Task<IActionResult> DownloadFile(string path="")
		{
			var data = await _service.GetFileString(path);

            if (data!=null&&data.Content.Headers.ContentLength.HasValue)
            {
				Response.Headers.SetCommaSeparatedValues("Content-Length", data.Content.Headers.ContentLength.Value.ToString());
            }

            Stream stream = await data.Content.ReadAsStreamAsync();

            return File(stream, "application/octet-stream", Path.GetFileName(path),true);
        }

		[HttpGet]
		public async Task<IActionResult> GetImage(string path,bool isFull=false)
		{
			HttpResponseMessage? data =await _service.GetPhoto(path, isFull);

            if (isFull&& data!=null)
            {
				 Stream stream = await data.Content.ReadAsStreamAsync();

				return File(stream, "application/octet-stream", Path.GetFileName(path), true);
			}

            if (data!=null&&data.IsSuccessStatusCode)
            {
				byte[] imageBytes = await data.Content.ReadAsByteArrayAsync();

				return File(imageBytes, "image/webp");

			}

			return StatusCode(503);
		}

		public async Task<IActionResult> GetFilesJson(string path,int skip,int take)
		{
			var files = await _service.GetData(path, skip, take);
            if (files!=null)
            {
				return Json(files.Skip(1));
			}
			return StatusCode(503);
		}

		public async Task<IActionResult> GetPrevAndNextPathsForPhoto(string path)
		{
			var data = await _service.GetPrevAndNextPhotoLocation(path);

            if (data!=null)
            {
				return Json(data);
            }

			return StatusCode(500);

        }



		public async Task<IActionResult> GetPhotoInfo(string path)
		{
			var data = await _service.GetPhotoInfo(path);

			if (data != null)
			{
				return Json(data);
			}

			return StatusCode(500);

		}

		[SkipStatusCodePages]
		public async Task<IActionResult> CheckConnection()
		{
            if (await _service.CheckConnection())
            {
				return StatusCode(200);
            }

			return StatusCode(503);
		}

		[SkipStatusCodePages]
		public async Task<IActionResult> ScanForAvailibleHosts()
		{
			if (await _service.ScanForAvailibleHost())
			{
				return StatusCode(200);
			}

			return StatusCode(503);
		}

	}
}
