namespace HomeAssistant.Middlewares
{
	public static class IsDeletedOrExpiredCheckMiddlewareExtension
	{

		public static IApplicationBuilder UseIsDeletedOrExpiredCheck(
			this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<IsDeletedOrExpiredCheckMiddleware>();
		}
	}
}
