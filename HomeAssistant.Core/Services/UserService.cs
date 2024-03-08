using HomeAssistant.Core.Constants;
using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Services
{
	public class UserService : IUserService
	{
		private readonly UserManager<HomeAssistantUser> userManager;


		public UserService(UserManager<HomeAssistantUser> _userManager, RoleManager<IdentityRole> _roleManager)
		{
			userManager = _userManager;
		}

		async Task<IEnumerable<UserDetailsViewModel>> IUserService.GetAllUsers()
		{
			var users = await userManager.Users
				.AsNoTracking()
				.Where(x => !x.IsDeleted)
				.OrderByDescending(x => x.CreatedOn)
				.ToListAsync();

			List<UserDetailsViewModel> allUsers = new List<UserDetailsViewModel>();
			foreach (var u in users)
			{
				string roles = string.Join(", ", await userManager.GetRolesAsync(u));

				allUsers.Add(new UserDetailsViewModel()
				{
					Id = u.Id,
					FirstName = u.FirstName,
					LastName = u.LastName,
					Username = u.UserName,
					Email = u.Email,
					CreatedOn = u.CreatedOn.ToString(DataValidationConstants.DateTimeFormat),
					Roles = roles,
				});
			}

			return allUsers;
		}

		async Task<IEnumerable<UserDetailsViewModel>> IUserService.GetAllNotApprovedUsers()
		{
			var users = await userManager.Users
				.AsNoTracking()
				.Where(x => !x.IsDeleted)
				.OrderByDescending(u => u.CreatedOn)
				.ToListAsync();

			List<UserDetailsViewModel> watingUsers = new List<UserDetailsViewModel>();
			foreach (var u in users)
			{
				string roles = string.Join(", ", await userManager.GetRolesAsync(u));

				if (roles == string.Empty)
				{
					watingUsers.Add(new UserDetailsViewModel()
					{
						Id = u.Id,
						FirstName = u.FirstName,
						LastName = u.LastName,
						Username = u.UserName,
						Email = u.Email,
						CreatedOn = u.CreatedOn.ToString(DataValidationConstants.DateTimeFormat),
						Roles = roles,
					});

				}

			}

			return watingUsers;

		}

		public async Task<IEnumerable<UserDetailsViewModel>> GetAllDeletedUsers()
		{
			var users = await userManager.Users
				.AsNoTracking()
				.Where(u => u.IsDeleted)
				.OrderByDescending(u=>u.DeletedOn)
				.Select(u=> new UserDetailsViewModel()
				{
					Id = u.Id,
					FirstName = u.FirstName,
					LastName = u.LastName,
					Username = u.UserName,
					Email = u.Email,
					CreatedOn = u.CreatedOn.ToString(DataValidationConstants.DateTimeFormat),
					DeletedOn=u.DeletedOn.Value.ToString(DataValidationConstants.DateTimeFormat)
				})
				.ToListAsync();

			return users;
		}

		public async Task<bool> ApproveById(string Id)
		{
			var user = await userManager.Users
				.FirstOrDefaultAsync(u => u.Id == Id);

			if (user == null)
			{
				return false;
			}

			await userManager.AddToRoleAsync(user, "NormalUser");
			return true;
		}

		public async Task<bool> DeleteById(string Id)
		{
			var user=await userManager.Users
				.Where(x=>!x.IsDeleted)
				.FirstOrDefaultAsync(x => x.Id == Id);

            if (user==null)
            {
				return false;
            }

			user.IsDeleted = true;
			user.DeletedOn = DateTime.Now;

			await userManager.UpdateAsync(user);
			return true;
		}

		public async Task<bool> RestoreById(string Id)
		{

			var user = await userManager.Users
				.Where(x => x.IsDeleted)
				.FirstOrDefaultAsync(x => x.Id == Id);

			if (user == null)
			{
				return false;
			}

			user.IsDeleted = false;
			user.DeletedOn = null;

			await userManager.UpdateAsync(user);
			return true;
		}
	}
}
