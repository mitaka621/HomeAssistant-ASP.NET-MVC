using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.SeedDb
{
    internal class Seeder
    {
        string adminGUID = "e2246145-9dd8-4902-ae41-68096b5ca738";
        string adminRoleGUID = "f23e50cd-3de0-4420-ae9b-6ce529f3128f";
        string normalUserGUID = "7e2d4805-c978-4600-9663-a9cafa2a54be";

        public HomeAssistantUser CreateAdminUser()
        {
            var hasher = new PasswordHasher<HomeAssistantUser>();

            var admin = new HomeAssistantUser()
            {
                Id = adminGUID,
                ConcurrencyStamp= adminGUID,
                UserName = "admin",
                Email = "admin",
                NormalizedUserName = "admin",
                NormalizedEmail = "admin"
            };

            admin.PasswordHash = hasher.HashPassword(admin, "admin123");

            return admin;
        }

        public IdentityRole[] CreateRoles()
        {
            return new IdentityRole[] 
            { 
                new IdentityRole()
                {
                    Name="NormalUser",
                    Id=normalUserGUID,
                    ConcurrencyStamp=normalUserGUID,               
                },
                new IdentityRole() 
                { 
                    Id = adminRoleGUID,
                    ConcurrencyStamp=adminRoleGUID,
                    Name= "Admin",
                } 
            };
        }

        public IdentityUserRole<string> AssignRoleToAdmin()
        {
            return new IdentityUserRole<string>()
            {
                UserId = adminGUID,
                RoleId = adminRoleGUID
            };
        }
    }
}
