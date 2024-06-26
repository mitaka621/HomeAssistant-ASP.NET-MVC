﻿using HomeAssistant.Core.Models;
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

		Task<FailedLoginPaginationViewModel> GetAllFailedLogins(int page=1,int countOnPage=30);

		Task ResetFailedLoginCount(string ip);

		Task<string> GetIpDetailsString(string ip);

		Task<UsersInteractionsPaginationViewModel> GetAllUsersInteractions(int page, int countOnPage=50);

		Task<GroupByControllerUserInteractionPaginationModel> GetAllUsersInteractionsGroupByController(int page, int countOnPage = 50);

		Task<UsersInteractionsPaginationViewModel> GetInteractionsForUser(string userId,int page, int countOnPage = 50);

		Task<IEnumerable<UserDetailsViewModel>> SearchForUser(string keyphrase);
	}
}
