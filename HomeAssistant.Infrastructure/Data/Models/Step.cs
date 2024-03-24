using HomeAssistant.Infrastructure.Data.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class Step
	{       
        public int RecipeId { get; set; }
        [ForeignKey(nameof(RecipeId))]
        public Recipe Recipe { get; set; } = null!;

        public int StepNumber { get; set; }

        [Required]
        [MaxLength(Constants.NameMaxLenght)]
        public string Name { get; set; }=string.Empty;

		[Required]
		[MaxLength(Constants.DescriptionMaxLength)]
		public string Description { get; set; } = string.Empty;

        [Required]
        public StepType StepType { get; set; }

        public int? DurationInMin { get; set; }

		public IEnumerable<RecipeProduct> RecipeProducts { get; set; } = new List<RecipeProduct>();
	}
}
