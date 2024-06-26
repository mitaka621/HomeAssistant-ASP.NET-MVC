using HomeAssistant.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeAssistant.Controllers
{
	[Authorize(Roles ="StandardUser")]
	public class MessageController : Controller
	{
		private readonly IUserService _userService;
		private readonly IMessageService _messageService;

        public MessageController(IUserService userService, IMessageService messageService)
        {
			_userService=userService;
			_messageService=messageService;
		}
        public async Task<IActionResult> Index()
		{
			return View((await _userService.GetAllApprovedNotDeletedUsersAsync()).Where(x=>x.Id!=GetUserId()));
		}

		public async Task<IActionResult> Chat(string recipiantId)
		{
			try
			{
				return View(await _messageService.GetChatDetails(GetUserId(), recipiantId,20));
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
			
		}

		public async Task<IActionResult> LoadMessageRangeJson(int chatroomId, int skip, int take)
		{
			return Json(await _messageService.LoadMessagesRange(chatroomId, skip, take));
		}

		private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
