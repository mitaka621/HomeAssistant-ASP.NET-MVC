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

        Task<IEnumerable<UserDetailsViewModel>> GetAllNotApprovedUsers();
    }
}
