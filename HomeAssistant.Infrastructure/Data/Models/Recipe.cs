using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class Recipe
	{
		[Comment("Recipe identifier")]
		[Key]
        public int Id { get; set; }

		[Comment("Recipe Name")]
		[Required]
		[MaxLength(Constants.NameMaxLenght)]
		public string Name { get; set; } = string.Empty;

		[Comment("Recipe Description")]
		[Required]
		[MaxLength(Constants.DescriptionMaxLength)]
		public string Description { get; set; } = string.Empty;

        public IEnumerable<RecipeProduct> RecipeProducts { get; set; } = new List<RecipeProduct>();

		public IEnumerable<Step> Steps { get; set; } = new List<Step>();

	}
}
