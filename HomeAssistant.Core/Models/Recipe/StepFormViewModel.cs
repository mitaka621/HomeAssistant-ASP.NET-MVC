using System.ComponentModel.DataAnnotations;
using static HomeAssistant.Core.Constants.ErrorMessages;
using static HomeAssistant.Core.Constants.DataValidationConstants;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.Core.Models.Recipe
{
	public class StepFormViewModel
	{
		[Required(ErrorMessage = RequiredField)]
		[StringLength(NameMaxLenght,
		   MinimumLength = NameMinLenght,
		   ErrorMessage = InvalidStringLength)]
		[Comment("Product name")]
		public string Name { get; set; } = string.Empty;

		[Required(ErrorMessage = RequiredField)]
		[StringLength(DescriptionMaxLength,
		   MinimumLength = DescriptionMinLength,
		   ErrorMessage = InvalidStringLength)]
		[Comment("Product name")]
		public string Description { get; set; } = string.Empty;

		[Range(0,int.MaxValue,ErrorMessage =ShouldBeGreaterThanZero)]
		[Display(Name="Duration in minutes:")]
        public int? Duration { get; set; }
    }
}
