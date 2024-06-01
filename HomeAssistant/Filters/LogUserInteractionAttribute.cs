using HomeAssistant.Core.Models.User;
using HomeAssistant.Hubs;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
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
		private readonly IHubContext<UsersActiviryHub> _usersActivityHub;

		public LogUserInteractionAttribute(ILogger<LogUserInteractionAttribute> logger, HomeAssistantDbContext dbContext, IHubContext<UsersActiviryHub> usersActivityHub)
        {
			_logger=logger;
			_dbContext=dbContext;
			_usersActivityHub=usersActivityHub;
		}

		public override async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
		{
			if (!IsLoggingDisabled(filterContext))
			{
				var model=await LogInteraction(filterContext);

				await _usersActivityHub.Clients
					.All
					.SendAsync("PushNewLogEntry", new UserInteractionViewModel() 
					{
						ActionArgumentsJson=model.ActionArgumentsJson,
						DateTime=model.DateTime,
						QueryString=model.QueryString,
						RequestType=model.RequestType,
						RequestUrl=model.RequestUrl,
						UserId=model.UserId,
						UserName=model.User.UserName,
					});
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

		private async Task<UserActivityLog> LogInteraction(ActionExecutingContext filterContext)
		{
			var request = filterContext.HttpContext.Request;

			var model = new UserActivityLog()
			{
				ActionArgumentsJson = JsonSerializer.Serialize(filterContext.ActionArguments),
				DateTime = DateTime.Now,
				QueryString = request.QueryString.ToString(),
				RequestType = request.Method,
				RequestUrl = request.Path.ToString(),
				UserId = GetUserId(filterContext)
			};

			_dbContext.UserActivityLogs.Add(model);

			await _dbContext.SaveChangesAsync();

			return model;
		}

		private string GetUserId(ActionExecutingContext filterContext)
		{
			return filterContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
