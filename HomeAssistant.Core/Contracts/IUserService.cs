using HomeAssistant.Core.Models;
using HomeAssistant.Core.Models.User;

namespace HomeAssistant.Core.Contracts
{
	public interface IUserService
    {
        Task<IEnumerable<UserDetailsViewModel>> GetAllNonDelitedUsersAsync();

		Task<IEnumerable<UserDetailsViewModel>> GetAllDeletedUsersAsync();

		Task<IEnumerable<UserDetailsViewModel>> GetAllNotApprovedUsersAsync();

		Task<IEnumerable<UserDetailsViewModel>> GetAllApprovedNotDeletedUsersAsync();

		Task<UserDetailsFormViewModel> GetUserByIdAsync(string Id);

		Task<IEnumerable<RoleViewModel>> GetAllRolesAsync();

		Task<bool> EditUserByIdAsync(string Id, UserDetailsFormViewModel user);

		Task<int> AddRoleToUser(string userId, string roleId);

		Task<bool> RemoveRoleFromUser(string userId, string role);

		Task<bool> ApproveByIdAsync(string Id);

		Task<bool> DeleteByIdAsync(string Id);

		Task<bool> RestoreByIdAsync(string Id);

		Task<bool> AddUserLocation(string Id,double latitude, double longitude);

		Task<IEnumerable<string>> GetAllApprovedNotDeletedUsersIds();

		Task<FailedLoginPaginationViewModel> GetAllFailedLogins(int page=1,int countOnPage=20);

		Task ResetFailedLoginCount(string ip);
	}
}
