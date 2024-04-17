using HomeAssistant.Infrastructure.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class Step
	{
        [Comment("Recipe step identifier")]
        public int RecipeId { get; set; }
        [ForeignKey(nameof(RecipeId))]
        public Recipe Recipe { get; set; } = null!;

        [Comment("Step number for the current recipe")]
        public int StepNumber { get; set; }

        [Comment("Step name")]
        [Required]
        [MaxLength(Constants.NameMaxLenght)]
        public string Name { get; set; }=string.Empty;

        [Comment("Step description")]
		[Required]
		[MaxLength(Constants.DescriptionMaxLength)]
		public string Description { get; set; } = string.Empty;

        [Comment("Type of recipe step - timer or task")]
        [Required]
        public StepType StepType { get; set; }

        [Comment("Duration for the current step if it is a timer")]
        public int? DurationInMin { get; set; }

		public IEnumerable<RecipeProductStep> RecipeProductStep { get; set; } = new List<RecipeProductStep>();
	}
}
