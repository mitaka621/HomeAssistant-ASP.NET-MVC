using HomeAssistant.Core.Constants;
using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models;
using HomeAssistant.Core.Models.User;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace HomeAssistant.Core.Services
{
	public class UserService : IUserService
	{
		private readonly UserManager<HomeAssistantUser> userManager;
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly HomeAssistantDbContext _dbContext;
		private readonly IimageService imageService;
		private readonly IConfiguration _configuration;
		private readonly ILogger<IUserService> _logger;

		public UserService(UserManager<HomeAssistantUser> _userManager, RoleManager<IdentityRole> _roleManager, IimageService imageService, HomeAssistantDbContext dbContext, IConfiguration configuration, ILogger<IUserService> logger)
		{
			userManager = _userManager;
			roleManager = _roleManager;
			this.imageService = imageService;
            _dbContext= dbContext;
			_configuration=configuration;
			_logger=logger;
		}

		public async Task<IEnumerable<UserDetailsViewModel>> GetAllNonDelitedUsersAsync()
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
					Ip = u.ClientIpAddress
				});
			}

			return allUsers;
		}

		public async Task<IEnumerable<UserDetailsViewModel>> GetAllNotApprovedUsersAsync()
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
						Ip=u.ClientIpAddress,
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
					DeletedOn = u.DeletedOn.Value.ToString(DataValidationConstants.DateTimeFormat),
					IsTempUser=u.ExpiresOn!=null
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
				AllRoles = await GetAllRolesAsync(),
				Latitude = user.Latitude,
				Longitude = user.Longitude
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
			var dbUser = userManager.Users.FirstOrDefault(x => x.Id == Id && !x.IsDeleted);

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

			if (role == null || user == null)
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
			if (user == null || !await userManager.IsInRoleAsync(user, role))
			{
				return false;
			}

			return (await userManager.RemoveFromRoleAsync(user, role)).Succeeded;
		}

		public async Task<bool> AddUserLocation(string Id, double latitude, double longitude)
		{
			var dbUser = userManager.Users.FirstOrDefault(x => x.Id == Id && !x.IsDeleted);

			if (dbUser == null)
			{
				return false;
			}

			dbUser.Latitude = latitude;
			dbUser.Longitude = longitude;

			return (await userManager.UpdateAsync(dbUser)).Succeeded;
		}

		public async Task<IEnumerable<UserDetailsViewModel>> GetAllApprovedNotDeletedUsersAsync()
		{
			var users = (await userManager
				.GetUsersInRoleAsync("StandardUser"))
				.Where(x => !x.IsDeleted)
				.Select(x => new UserDetailsViewModel()
				{
					Id = x.Id,
					FirstName = x.FirstName,
					LastName = x.LastName,
					Email = x.Email,
					Username = x.UserName,
				}).ToList();


			var userPhotos = await imageService
				.GetPfpRange(users.Select(x => x.Id).Distinct().ToArray());

			users
				.ForEach(x => x.Photo = userPhotos.FirstOrDefault(y => y.Key == x.Id).Value ?? new byte[0]);

			return users;

		}

		public async Task<IEnumerable<string>> GetAllApprovedNotDeletedUsersIds()
		{
			return (await userManager
				.GetUsersInRoleAsync("StandardUser"))
				.Where(x => !x.IsDeleted)
				.Select(x => x.Id);
		}

        public async Task<FailedLoginPaginationViewModel> GetAllFailedLogins(int page = 1, int countOnPage = 20)
        {
			int totalRecordsCount=await _dbContext.BlacklistedIPs.CountAsync();

			int maxPages= (int)Math.Ceiling(totalRecordsCount/(double)countOnPage);

            if (maxPages==0)
            {
				maxPages = 1;

			}

            if (page<1)
            {
				page = 1;
            }
            else if(page >= maxPages)
            {
                page=maxPages;
            }

            int recordsToSkip = (page - 1) * countOnPage;

            var data=await _dbContext.BlacklistedIPs
			.OrderByDescending(x => x.LastTry)
			.Where(x=>x.Count!=0)
			.Select(x => new FailedLoginViewModel()
			{
				AttemptsCount = x.Count,
				Ip = x.Ip,
				LastAttemptOn = x.LastTry
			})
			.Skip(recordsToSkip)
			.Take(countOnPage)			
			.ToListAsync();



			return new FailedLoginPaginationViewModel()
			{
				CurrentPage = page,
				Records=data,
				RecordsOnPage= countOnPage,
				TotalPages= maxPages
			};
        }

		public async Task ResetFailedLoginCount(string ip)
		{
			var record=await _dbContext.BlacklistedIPs.FirstOrDefaultAsync(x => x.Ip == ip);

            if (record==null)
            {
				throw new ArgumentNullException(nameof(record));
            }

			record.Count= 0;

			await _dbContext.SaveChangesAsync();

		}

		public async Task<string> GetIpDetailsString(string ip)
		{
			var client = new HttpClient();

			try
			{
				string json=await client.GetStringAsync($"https://ipinfo.io/{ip}?token={_configuration.GetSection("ExternalServiceApiKeys")["ipinfo"]}");

				return json;
			}
			catch (Exception e)
			{
				_logger.LogWarning("Failed to retrieve ip details: " + e.Message);
				throw new HttpRequestException(e.Message, e);
			}
			
		}

		public async Task<UsersInteractionsPaginationViewModel> GetAllUsersInteractions(int page, int countOnPage = 50)
		{
			int totalRecordsCount = await _dbContext.UserActivityLogs.CountAsync();

			int maxPages = (int)Math.Ceiling(totalRecordsCount / (double)countOnPage);

			if (page < 1)
			{
				page = 1;
			}
			else if (page >= maxPages)
			{
				page = maxPages;
			}

			int recordsToSkip = (page - 1) * countOnPage;

			var data = await _dbContext.UserActivityLogs
			.OrderByDescending(x => x.DateTime)
			.Select(x => new UserInteractionViewModel()
			{
				ActionArgumentsJson = x.ActionArgumentsJson,
				DateTime= x.DateTime,
				QueryString = x.QueryString,
				RequestType= x.RequestType,
				RequestUrl= x.RequestUrl,
				UserId= x.UserId,
				UserName=x.User.UserName
			})
			.Skip(recordsToSkip)
			.Take(countOnPage)
			.ToListAsync();

			return new UsersInteractionsPaginationViewModel()
			{
				CurrentPage = page,
				Interactions = data,
				RecordsOnPage = countOnPage,
				TotalPages = maxPages
			};
		}
	}
}
