using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class RecipeProductStep
	{
		[Comment("Recipe id")]
		public int RecipeId { get; set; }

		[Comment("Product id")]
		public int ProductId { get; set; }
		[ForeignKey("RecipeId, ProductId")]
		public RecipeProduct RecipeProduct { get; set; } = null!;

		[Comment("Recipe step number for which the recipe product is associated with")]
		public int StepNumber { get; set; }
		[ForeignKey("RecipeId, StepNumber")]
		public Step Step { get; set; } = null!;
	}
}
