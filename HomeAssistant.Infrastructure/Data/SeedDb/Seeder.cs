﻿using HomeAssistant.Infrastructure.Data.Models;
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
		string standardUserGUID = "7e2d4805-c978-4600-9663-a9cafa2a54be";

		public HomeAssistantUser CreateAdminUser()
		{
			var hasher = new PasswordHasher<HomeAssistantUser>();

			var admin = new HomeAssistantUser()
			{
				Id = adminGUID,
				ConcurrencyStamp = adminGUID,
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
					Name="StandardUser",
					NormalizedName = "StandardUser",
					Id=standardUserGUID,
					ConcurrencyStamp=standardUserGUID,
				},
				new IdentityRole()
				{
					Name= "Admin",
					NormalizedName = "Admin",
					Id = adminRoleGUID,
					ConcurrencyStamp=adminRoleGUID,
				}
			};
		}

		public IdentityUserRole<string>[] AssignRoleToAdmin()
		{
			return new IdentityUserRole<string>[]
			{
				new IdentityUserRole<string>()
				{
					UserId = adminGUID,
					RoleId = adminRoleGUID
				},
				new IdentityUserRole<string>()
				{
					UserId = adminGUID,
					RoleId = standardUserGUID
				}
			};
		}

		public Category[] CreateCategories()
		{
			return new Category[]
			{
				new Category()
				{
					Id = 1,
					Name = "Dairy"
				},
				new Category()
				{
					Id = 2,
					Name = "Fruits"
				},
				new Category()
				{
					Id = 3,
					Name = "Meat and Poultry"
				},
				new Category()
				{
					Id = 4,
					Name = "Beverages"
				},
				new Category()
				{
					Id = 5,
					Name = "Vegetables"
				},
				new Category()
				{
					Id = 6,
					Name = "Baking Supplies"
				},
				new Category()
				{
					Id = 7,
					Name = "Condiments and Sauces"
				}
			};
		}

		public Product[] CreateProducts()
		{
			return new Product[]
			{
				new Product()
				{
					Id = 1,
					Name="Steak",
					CategoryId=3,
					Count=3,
					AddedOn=new DateTime(2024,3,14)
				},
				new Product()
				{
					Id = 2,
					Name="Apple",
					CategoryId=2,
					Count=10,
					AddedOn=new DateTime(2024,3,14)
				},
			};
		}
	}
}
