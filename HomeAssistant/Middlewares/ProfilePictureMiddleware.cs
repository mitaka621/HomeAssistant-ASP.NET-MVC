﻿using HomeAssistant.Core.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace HomeAssistant.Middlewares
{
    [Obsolete("ProfilePictureMiddleware is deprecated")]
    public class ProfilePictureMiddleware
    {
        private readonly RequestDelegate _next;

        public ProfilePictureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IImageService _ImageService, IMemoryCache _cache)
        {
            if (!_cache.TryGetValue(GetUserId(context), out byte[] profilePicture))
            {
                profilePicture = await _ImageService.GetPFP(GetUserId(context));
                _cache.Set(GetUserId(context), profilePicture, TimeSpan.FromMinutes(1));
            }

            context.Items["ProfilePicture"] = profilePicture;

            await _next(context);
        }

        private string GetUserId(HttpContext context)
        {
            return context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
