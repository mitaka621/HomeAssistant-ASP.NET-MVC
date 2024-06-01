using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using static MongoDB.Driver.WriteConcern;

namespace HomeAssistant.Hubs
{
	[Authorize(Roles = "Admin")]
	public class UsersActiviryHub : Hub
	{

	}

}
