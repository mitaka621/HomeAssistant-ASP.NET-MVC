namespace HomeAssistant.Middlewares
{
	public static class ProfilePictureMiddlewareExtension
	{
		public static IApplicationBuilder UseProfilePicture(
			this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ProfilePictureMiddleware>();
		}
	}
}
