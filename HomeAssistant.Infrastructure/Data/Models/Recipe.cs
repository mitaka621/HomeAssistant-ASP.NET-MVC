using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class Recipe
	{
		[Key]
        public int Id { get; set; }

		[Required]
		[MaxLength(Constants.NameMaxLenght)]
		public string Name { get; set; } = string.Empty;

		[Required]
		[MaxLength(Constants.DescriptionMaxLength)]
		public string Description { get; set; } = string.Empty;

        public IEnumerable<RecipeProduct> RecipeProducts { get; set; } = new List<RecipeProduct>();

		public IEnumerable<Step> Steps { get; set; } = new List<Step>();

	}
}
