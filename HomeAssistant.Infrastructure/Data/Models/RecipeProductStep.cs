using System.ComponentModel.DataAnnotations.Schema;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class RecipeProductStep
	{
		public int RecipeId { get; set; }
		public int ProductId { get; set; }
		[ForeignKey("RecipeId, ProductId")]
		public RecipeProduct Recipe { get; set; } = null!;

		public int StepNumber { get; set; }
		[ForeignKey("RecipeId, StepNumber")]
		public Step Step { get; set; } = null!;
	}
}
