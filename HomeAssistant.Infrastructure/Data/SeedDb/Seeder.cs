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

        public HomeAssistantUser CreateAdminUser()
        {
            var hasher = new PasswordHasher<HomeAssistantUser>();

            var admin = new HomeAssistantUser()
            {
                Id = adminGUID,
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
            return new IdentityRole[] { new IdentityRole("NormalUser"), new IdentityRole("Admin") { Id = adminRoleGUID } };
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
