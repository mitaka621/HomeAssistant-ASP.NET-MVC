using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text.Json;

namespace HomeAssistant.Filters
{
	public class LogUserInteractionAttribute:ActionFilterAttribute
	{
		private readonly ILogger _logger;
		private readonly HomeAssistantDbContext _dbContext;

        public LogUserInteractionAttribute(ILogger<LogUserInteractionAttribute> logger, HomeAssistantDbContext dbContext)
        {
			_logger=logger;
			_dbContext=dbContext;
		}

		public override async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
		{
			if (!IsLoggingDisabled(filterContext))
			{
				await LogInteraction(filterContext);
			}

			await next();
		}

		private bool IsLoggingDisabled(ActionExecutingContext filterContext)
		{
			try
			{
				var controllerType = filterContext.Controller.GetType();
				var actionMethod = controllerType
					.GetMethod((filterContext.ActionDescriptor as ControllerActionDescriptor)!.ActionName)!;

				var controllerAttributes = controllerType.GetCustomAttributes(typeof(NoUserLoggingAttribute), true);

				var actionAttributes = actionMethod.GetCustomAttributes(typeof(NoUserLoggingAttribute), true);

				if (actionAttributes.Any() || controllerAttributes.Any())
				{
					return true;
				}

				return false;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Cant get action attributes");
				return false;
			}

            
		}

		private async Task LogInteraction(ActionExecutingContext filterContext)
		{
			var request = filterContext.HttpContext.Request;

			_dbContext.UserActivityLogs.Add(new UserActivityLog()
			{
				ActionArgumentsJson = JsonSerializer.Serialize(filterContext.ActionArguments),
				DateTime = DateTime.Now,
				QueryString = request.QueryString.ToString(),
				RequestType = request.Method,
				RequestUrl = request.Path.ToString(),
				UserId = GetUserId(filterContext)
			});

			await _dbContext.SaveChangesAsync();
		}

		private string GetUserId(ActionExecutingContext filterContext)
		{
			return filterContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
