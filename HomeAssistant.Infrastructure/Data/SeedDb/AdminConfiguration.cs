using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.SeedDb
{
    internal class AdminConfiguration : IEntityTypeConfiguration<HomeAssistantUser>
    {
        public void Configure(EntityTypeBuilder<HomeAssistantUser> builder)
        {
            var data = new Seeder();

            builder.HasData(data.CreateAdminUser());
        }
    }
}
