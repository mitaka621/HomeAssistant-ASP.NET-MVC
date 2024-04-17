using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class UserStep
	{
		[Comment("Recipe identifier")]
		public int RecipeId { get; set; }

		[Comment("User Identifier")]
		public string UserId { get; set; }=string.Empty;
		[ForeignKey(nameof(UserId))]
		public HomeAssistantUser User { get; set; } = null!;

		[Comment("Step number on which the user is currently on for the given recipe")]
		public int StepNumber { get; set; }
		[ForeignKey("RecipeId, StepNumber")]
		public Step Step { get; set; }=null!;

		[Comment("When was this recipe step started")]
        public DateTime StartedOn { get; set; }
    }
}
