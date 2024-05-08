﻿using HomeAssistant.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;

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

            if (data!=null&&data.Content.Headers.ContentLength.HasValue)
            {
				Response.Headers.SetCommaSeparatedValues("Content-Length", data.Content.Headers.ContentLength.Value.ToString());
            }

            Stream stream = await data.Content.ReadAsStreamAsync();

            return File(stream, "application/octet-stream", Path.GetFileName(path),true);
        }

		[HttpGet]
		public async Task<IActionResult> GetImage(string path)
		{
			Stopwatch sw = Stopwatch.StartNew();
			var data = await _service.GetPhoto(path);
			
            if (data!=null&&data.IsSuccessStatusCode)
            {
				byte[] imageBytes = await data.Content.ReadAsByteArrayAsync();

				sw.Stop();
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

    }
}
