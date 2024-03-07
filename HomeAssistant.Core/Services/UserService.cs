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
        private readonly RoleManager<IdentityRole> roleManager;

        public UserService(UserManager<HomeAssistantUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            userManager = _userManager;
            roleManager = _roleManager;

        }

        async Task<IEnumerable<UserDetailsViewModel>> IUserService.GetAllNotApprovedUsers()
        {
            var users = await userManager.Users.AsNoTracking().ToListAsync();

            List<UserDetailsViewModel> watingUsers=new List<UserDetailsViewModel>();
            foreach (var u in users)
            {
              string roles= string.Join(", ",await userManager.GetRolesAsync(u));

                if (roles==string.Empty)
                {
                    watingUsers.Add(new UserDetailsViewModel()
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Username = u.UserName,
                        Email=u.Email,
                        CreatedOn=u.CreatedOn.ToString(DataValidationConstants.DateTimeFormat),
                        Roles = roles,
                    });

                }
                
            }

            return watingUsers.OrderByDescending(x=>x.CreatedOn);

        }

        Task<IEnumerable<UserDetailsViewModel>> IUserService.GetAllUsers()
        {
            throw new NotImplementedException();
        }
    }
}
