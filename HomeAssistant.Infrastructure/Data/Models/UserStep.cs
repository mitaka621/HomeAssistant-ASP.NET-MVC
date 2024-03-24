using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class UserStep
	{
		public int RecipeId { get; set; }

		public string UserId { get; set; }=string.Empty;
		[ForeignKey(nameof(UserId))]
		public HomeAssistantUser User { get; set; } = null!;

		public int StepNumber { get; set; }
		[ForeignKey("RecipeId, StepNumber")]
		public Step Step { get; set; }=null!;

        public DateTime StartedOn { get; set; }
    }
}
