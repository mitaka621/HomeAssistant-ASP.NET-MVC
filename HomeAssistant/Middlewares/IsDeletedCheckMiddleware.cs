﻿using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace HomeAssistant.Middlewares
{
	public class IsDeletedOrExpiredCheckMiddleware
	{
		private readonly RequestDelegate _next;

		public IsDeletedOrExpiredCheckMiddleware(RequestDelegate next)
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

            if (user != null&&user.ExpiresOn!=null)
            {
                if (user.ExpiresOn < DateTime.Now)
                {
					user.IsDeleted = true;
					user.DeletedOn = user.ExpiresOn;

					await userManager.UpdateAsync(user);

                    await context.SignOutAsync(IdentityConstants.ApplicationScheme);
                }
            }                     

            // Continue with the request pipeline
            await _next(context);
		}
	}
}
