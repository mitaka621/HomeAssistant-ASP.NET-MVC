using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Services;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace HomeAssistant.Middlewares
{
	public class ProfilePictureMiddleware
	{
		private readonly RequestDelegate _next;

		public ProfilePictureMiddleware(RequestDelegate next)
		{
			_next = next;	
		}

		public async Task Invoke(HttpContext context, UserManager<HomeAssistantUser> userManager, IimageService _ImageService)
		{
			// Fetch the profile picture
			var user = await userManager.GetUserAsync(context.User);
            if (user==null)
            {
				await _next(context);
				return;
			}

            byte[] profilePicture = await _ImageService.GetPFP(user.Id);

			// Store the profile picture in the HttpContext.Items
			context.Items["ProfilePicture"] = profilePicture;

			// Continue processing the request pipeline
			await _next(context);
		}
	}
}
