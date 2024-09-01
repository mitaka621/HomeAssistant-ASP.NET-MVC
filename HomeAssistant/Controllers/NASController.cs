using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Services;
using HomeAssistant.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HomeAssistant.Controllers
{
	[Authorize(Roles = "NASUser")]
	public class NASController : Controller
	{
		private readonly INASHostService _service;
		public NASController(INASHostService service)
		{
			_service = service;
		}

		public async Task<IActionResult> Index(string path = "")
		{
			ViewBag.Path = path;

			var data = await _service.GetData(path);


			if (data == null)
			{
				return StatusCode(503);
			}

			ViewBag.currentHostIP = NASHostService.currentHostIp;
			return View(data);
		}

		public async Task<IActionResult> DownloadFile(string path = "")
		{
			var data = await _service.GetFileString(path);

			if (data != null && data.Content.Headers.ContentLength.HasValue)
			{
				Response.Headers.SetCommaSeparatedValues("Content-Length", data.Content.Headers.ContentLength.Value.ToString());
			}

			Stream stream = await data.Content.ReadAsStreamAsync();

			return File(stream, "application/octet-stream", Path.GetFileName(path), true);
		}

		[HttpGet]
		[NoUserLogging]
		public async Task<IActionResult> GetImage(string path, bool isFull = false)
		{
			HttpResponseMessage? data = await _service.GetPhoto(path, isFull);

			if (isFull && data != null)
			{
				Stream stream = await data.Content.ReadAsStreamAsync();

				return File(stream, "application/octet-stream", Path.GetFileName(path), true);
			}

			if (data != null && data.IsSuccessStatusCode)
			{
				byte[] imageBytes = await data.Content.ReadAsByteArrayAsync();

				return File(imageBytes, "image/webp");

			}

			return StatusCode(503);
		}

		[SkipStatusCodePages]
		[NoUserLogging]
		public async Task<IActionResult> GetFilesJson(string path, int skip, int take)
		{
			var files = await _service.GetData(path, skip, take);
			if (files != null)
			{
				return Json(files.Skip(1));
			}
			return StatusCode(503);
		}

		[SkipStatusCodePages]
		[NoUserLogging]
		public async Task<IActionResult> GetPrevAndNextPathsForPhoto(string path)
		{
			var data = await _service.GetPrevAndNextPhotoLocation(path);

			if (data != null)
			{
				return Json(data);
			}

			return StatusCode(500);

		}

		[SkipStatusCodePages]
		[NoUserLogging]
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
		[NoUserLogging]
		public async Task<IActionResult> CheckConnection()
		{
			if (await _service.CheckConnection())
			{
				return StatusCode(200);
			}

			return StatusCode(503);
		}

		[SkipStatusCodePages]
		[NoUserLogging]
		public async Task<IActionResult> ScanForAvailibleHosts()
		{
			if (await _service.ScanForAvailibleHost())
			{
				return StatusCode(200);
			}

			return StatusCode(503);
		}

		[SkipStatusCodePages]
		[NoUserLogging]
		public async Task<ActionResult> PlayVideo(string path)
		{
			if (!Request.Headers.ContainsKey("Range"))
			{
				return BadRequest();
			}

			var rangeHeader = Request.Headers["Range"].ToString();

			var data = await _service.GetVideoRange(path, rangeHeader);

			if (data != null && data.IsSuccessStatusCode)
			{
				Response.Headers.Add("Content-Range", string.Join("", data.Content.Headers.GetValues("Content-Range")));
				Response.Headers.Add("Accept-Ranges", "bytes");
				Response.Headers.Add("Content-Length", string.Join("", data.Content.Headers.GetValues("Content-Length")));
				Response.Headers.Add("Content-Type", string.Join("", data.Content.Headers.GetValues("Content-Type")));
				Response.StatusCode = (int)HttpStatusCode.PartialContent;

				return File(await data.Content.ReadAsByteArrayAsync(), "video/mp4");
			}

			return BadRequest();
		}

	}
}
