using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models;
using HomeAssistant.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeAssistant.Controllers
{
	[Authorize(Roles = "Admin")]
	public class UserConfigurationController : Controller
	{
		IUserService userService;
		IimageService _ImageService;
		public UserConfigurationController(IUserService _userService, IimageService ImageService)
		{
			userService = _userService;
			_ImageService = ImageService;
		}

		[HttpGet]
		public async Task<IActionResult> Index(string? ToastTitle, string? ToastMessage)
		{
			ViewBag.ToastTitle = ToastTitle;
			ViewBag.ToastMessage = ToastMessage;

			return View(await userService.GetAllNotApprovedUsersAsync());
		}

		[HttpGet]
		public async Task<IActionResult> All(string? ToastTitle, string? ToastMessage)
		{
			ViewBag.ToastTitle = ToastTitle;
			ViewBag.ToastMessage = ToastMessage;

			return View(await userService.GetAllNonDelitedUsersAsync());
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

			if (!await userService.ApproveByIdAsync(Id))
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
		[AutoValidateAntiforgeryToken]
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

			if (!await userService.DeleteByIdAsync(Id))
			{
				return RedirectToAction(nameof(All), new
				{
					ToastTitle = "Something went wrong",
					ToastMessage = "Failed to delete user!"
				});
			}

			return RedirectToAction(nameof(AllDeleted), new
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

			return View(await userService.GetAllDeletedUsersAsync());
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> RestoreById(string Id)
		{
			if (!await userService.RestoreByIdAsync(Id))
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

		[HttpGet]
		public async Task<IActionResult> Edit(string Id, string? ToastTitle, string? ToastMessage)
		{
			ViewBag.ToastTitle = ToastTitle;
			ViewBag.ToastMessage = ToastMessage;

			var imageData = await _ImageService.GetPFP(Id);

			if (imageData != null && imageData.Length > 0)
			{
				ViewBag.ImageData = imageData;
			}

			try
			{
                return View(await userService.GetUserByIdAsync(Id));
            }
			catch (ArgumentNullException) 
			{			
                return RedirectToAction(nameof(All), new
                {
                    ToastTitle = "Something went wrong",
                    ToastMessage = "Internal server error!"
                });
            }			
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> Edit(UserDetailsFormViewModel user)
		{
            if (!ModelState.IsValid)
			{
				user.AllRoles=await userService.GetAllRolesAsync();
				user.UserRoles =(await userService.GetUserByIdAsync(user.Id)).UserRoles;
				return View(user);
			}

			if (!await userService.EditUserByIdAsync(user.Id, user))
			{
				return RedirectToAction(nameof(All), new
				{
					ToastTitle = "Something went wrong",
					ToastMessage = "Try again later!"
				});
			}

			return RedirectToAction(nameof(Edit), new
			{
				user.Id,
				ToastTitle = "Success",
				ToastMessage = "User edited successfully!"
			});
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> AddRole(UserDetailsFormViewModel user)
		{
			int result = await userService.AddRoleToUser(user.Id, user.SelectedRoleId);
			if (result==-1)
			{
				return RedirectToAction(nameof(All), new
				{
					ToastTitle = "Something went wrong",
					ToastMessage = "Try again later!"
				});
			}

            if (result==0)
            {
				return RedirectToAction(nameof(Edit), new
				{
					user.Id,
					ToastTitle = "Error",
					ToastMessage = "The user already has that role!"
				});
			}

            return RedirectToAction(nameof(Edit), new
			{
				user.Id,
				ToastTitle = "Success",
				ToastMessage = "The role has been added successfully!"
			});
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> DeleteRole(UserDetailsFormViewModel user,string role)
		{
            if (!await userService.RemoveRoleFromUser(user.Id, role))
            {
				return RedirectToAction(nameof(All), new
				{
					ToastTitle = "Something went wrong",
					ToastMessage = "Try again later!"
				});
			}

			return RedirectToAction(nameof(Edit), new
			{
				user.Id,
				ToastTitle = "Success",
				ToastMessage = "The role has been removed successfully!"
			});
		}
		private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}

	}
}
