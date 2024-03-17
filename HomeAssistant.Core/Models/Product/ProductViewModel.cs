using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeAssistant.Core.Models.Product;

namespace HomeAssistant.Core.Models
{
    public class ProductViewModel
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public int Count { get; set; }

		public int? Weight { get; set; }

		public DateTime AddedOn { get; set; }

		public UserDetailsViewModel? User { get; set; }

		public CategoryViewModel ProductCategory { get; set; } = null!;

	}
}
