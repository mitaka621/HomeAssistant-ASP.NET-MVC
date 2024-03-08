using HomeAssistant.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Contracts
{
    public interface IUserService
    {
        Task<IEnumerable<UserDetailsViewModel>> GetAllUsers();

		Task<IEnumerable<UserDetailsViewModel>> GetAllDeletedUsers();

		Task<IEnumerable<UserDetailsViewModel>> GetAllNotApprovedUsers();

        Task<bool> ApproveById(string Id);

		Task<bool> DeleteById(string Id);

		Task<bool> RestoreById(string Id);
	}
}
