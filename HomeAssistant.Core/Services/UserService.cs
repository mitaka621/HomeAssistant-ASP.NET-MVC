using HomeAssistant.Core.Constants;
using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HomeAssistant.Core.Services
{
	public class UserService : IUserService
    {
        private readonly UserManager<HomeAssistantUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserService(UserManager<HomeAssistantUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            userManager = _userManager;
            roleManager = _roleManager;
        }

        async Task<IEnumerable<UserDetailsViewModel>> IUserService.GetAllUsersAsync()
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

        async Task<IEnumerable<UserDetailsViewModel>> IUserService.GetAllNotApprovedUsersAsync()
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

        public async Task<IEnumerable<UserDetailsViewModel>> GetAllDeletedUsersAsync()
        {
            var users = await userManager.Users
                .AsNoTracking()
                .Where(u => u.IsDeleted)
                .OrderByDescending(u => u.DeletedOn)
                .Select(u => new UserDetailsViewModel()
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.UserName,
                    Email = u.Email,
                    CreatedOn = u.CreatedOn.ToString(DataValidationConstants.DateTimeFormat),
                    DeletedOn = u.DeletedOn.Value.ToString(DataValidationConstants.DateTimeFormat)
                })
                .ToListAsync();

            return users;
        }

        public async Task<bool> ApproveByIdAsync(string Id)
        {
            var user = await userManager.Users
                .FirstOrDefaultAsync(u => u.Id == Id);

            if (user == null)
            {
                return false;
            }

			return (await userManager.AddToRoleAsync(user, "StandardUser")).Succeeded;           
        }

        public async Task<bool> DeleteByIdAsync(string Id)
        {
            var user = await userManager.Users
                .Where(x => !x.IsDeleted)
                .FirstOrDefaultAsync(x => x.Id == Id);

			if (user == null)
            {
                return false;
            }

			user.IsDeleted = true;
            user.DeletedOn = DateTime.Now;

			return (await userManager.UpdateAsync(user)).Succeeded;
           
        }

        public async Task<bool> RestoreByIdAsync(string Id)
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

			return (await userManager.UpdateAsync(user)).Succeeded;           
        }

        public async Task<UserDetailsFormViewModel> GetUserByIdAsync(string Id)
        {
            var user = await userManager.Users
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .FirstOrDefaultAsync(x => x.Id == Id);

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return new UserDetailsFormViewModel
            {
                Id = Id,
                Email = user.Email,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserRoles = await userManager.GetRolesAsync(user),
                AllRoles = await GetAllRolesAsync()
            };
        }

        public async Task<IEnumerable<RoleViewModel>> GetAllRolesAsync()
        {

            return await roleManager.Roles
                .AsNoTracking()
                .Select(x => new RoleViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToListAsync();
        }

		public async Task<bool> EditUserByIdAsync(string Id, UserDetailsFormViewModel user)
		{
           var dbUser= userManager.Users.FirstOrDefault(x => x.Id == Id && !x.IsDeleted);

			if (dbUser == null) 
            {
                return false;
            }

			dbUser.FirstName = user.FirstName;
            dbUser.LastName = user.LastName;
            dbUser.Email = user.Email;
            dbUser.UserName = user.Username;

			return (await userManager.UpdateAsync(dbUser)).Succeeded;		
		}

        /// <summary>
        /// Returns -1 if user or role are null, 0 if the user already has that role and 1 if the role was added succesfully
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
		public async Task<int> AddRoleToUser(string userId, string roleId)
		{
			
			var user = userManager.Users
                .FirstOrDefault(x => x.Id == userId && !x.IsDeleted);

            var role = roleManager.Roles.FirstOrDefault(x => x.Id == roleId);

            if (role == null||user==null) 
            {
                return -1;
            }

            if (await userManager.IsInRoleAsync(user, role.Name))
            {
                return 0;
            }

			return Convert.ToInt32((await userManager.AddToRoleAsync(user, role.Name)).Succeeded);
		}

		public async Task<bool> RemoveRoleFromUser(string userId, string role)
		{
			var user = userManager.Users
				.FirstOrDefault(x => x.Id == userId && !x.IsDeleted);
            if (user==null||!await userManager.IsInRoleAsync(user,role))
            {
                return false;
            }

			return (await userManager.RemoveFromRoleAsync(user, role)).Succeeded;           
		}
	}
}
