using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace HomeAssistant.Middlewares
{
	public class IsDeletedCheckMiddleware
	{
		private readonly RequestDelegate _next;

		public IsDeletedCheckMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context, UserManager<HomeAssistantUser> userManager)
		{
			// Check the "isDeleted" property and log out the user if true
			var user = await userManager.GetUserAsync(context.User);
			if (user != null && user.IsDeleted)
			{
				// Log out the user
					
				await context.SignOutAsync(IdentityConstants.ApplicationScheme);
			}

			// Continue with the request pipeline
			await _next(context);
		}
	}
}
