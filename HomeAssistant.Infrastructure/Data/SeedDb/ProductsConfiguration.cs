using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.SeedDb
{
	public class ProductsConfiguration : IEntityTypeConfiguration<Product>

	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			var data = new Seeder();
			builder.HasData(data.CreateProducts());
		}
	}
}
