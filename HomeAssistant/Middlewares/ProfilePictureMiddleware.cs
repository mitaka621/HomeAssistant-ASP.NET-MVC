using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Services;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace HomeAssistant.Middlewares
{
	public class ProfilePictureMiddleware
	{
		private readonly RequestDelegate _next;

		public ProfilePictureMiddleware(RequestDelegate next)
		{
			_next = next;	
		}

		public async Task Invoke(HttpContext context, IimageService _ImageService,IMemoryCache _cache)
		{
			if (!_cache.TryGetValue(GetUserId(context), out byte[] profilePicture))
			{
				profilePicture = await _ImageService.GetPFP(GetUserId(context));

				_cache.Set(GetUserId(context), profilePicture, TimeSpan.FromMinutes(10)); 

			}

			context.Items["ProfilePicture"] = profilePicture;

			await _next(context);
		}

		private string GetUserId(HttpContext context)
		{
			return context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
