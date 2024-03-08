using HomeAssistant.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeAssistant.Controllers
{
	[Authorize(Roles = "Admin")]
	public class UserConfigurationController : Controller
	{
		IUserService userService;
		public UserConfigurationController(IUserService _userService)
		{
			userService = _userService;
		}

		[HttpGet]
		public async Task<IActionResult> Index(string? ToastTitle, string? ToastMessage)
		{
			ViewBag.ToastTitle = ToastTitle;
			ViewBag.ToastMessage = ToastMessage;

			return View(await userService.GetAllNotApprovedUsers());
		}
		[HttpGet]
		public async Task<IActionResult> All(string? ToastTitle, string? ToastMessage)
		{
			ViewBag.ToastTitle = ToastTitle;
			ViewBag.ToastMessage = ToastMessage;

			return View(await userService.GetAllUsers());
		}

		[HttpPost]
		public async Task<IActionResult> ApproveById(string Id)
		{
			if (Id == GetUserId())
			{
				return RedirectToAction(nameof(Index), new
				{
					ToastTitle = "Error",
					ToastMessage = "You can't approve yourself!"
				});
			}

			if (!await userService.ApproveById(Id))
			{
				return RedirectToAction(nameof(Index), new
				{
					ToastTitle = "Something went wrong",
					ToastMessage = "Failed to approve user!"
				});
			}

			return RedirectToAction(nameof(Index), new
			{
				ToastTitle = "Success",
				ToastMessage = "Successfully approved user!"
			});
		}

		[HttpPost]
		public async Task<IActionResult> DeleteById(string Id)
		{
			if (Id == GetUserId())
			{
				return RedirectToAction(nameof(All), new
				{
					ToastTitle = "Error",
					ToastMessage = "You can't delete yourself!"
				});
			}

			if (!await userService.DeleteById(Id))
			{
				return RedirectToAction(nameof(All), new
				{
					ToastTitle = "Something went wrong",
					ToastMessage = "Failed to delete user!"
				});
			}

			return RedirectToAction(nameof(All), new
			{
				ToastTitle = "Success",
				ToastMessage = "User successfully deleted!"
			});
		}

		[HttpGet]
		public async Task<IActionResult> AllDeleted(string? ToastTitle, string? ToastMessage)
		{
			ViewBag.ToastTitle = ToastTitle;
			ViewBag.ToastMessage = ToastMessage;

			return View(await userService.GetAllDeletedUsers());
		}

		[HttpPost]
		public async Task<IActionResult> RestoreById(string Id)
		{
			if (!await userService.RestoreById(Id))
			{
				return RedirectToAction(nameof(AllDeleted), new
				{
					ToastTitle = "Something went wrong",
					ToastMessage = "Failed to restore user!"
				});
			}

			return RedirectToAction(nameof(AllDeleted), new
			{
				ToastTitle = "Success",
				ToastMessage = "User successfully restored!"
			});
		}


		private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}

	}
}
