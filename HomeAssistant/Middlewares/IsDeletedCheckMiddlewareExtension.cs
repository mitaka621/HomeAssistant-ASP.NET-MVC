namespace HomeAssistant.Middlewares
{
	public static class IsDeletedCheckMiddlewareExtension
	{

		public static IApplicationBuilder UseIsDeletedCheck(
			this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<IsDeletedCheckMiddleware>();
		}
	}
}
