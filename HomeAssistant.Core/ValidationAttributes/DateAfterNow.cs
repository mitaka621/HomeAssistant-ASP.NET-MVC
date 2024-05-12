using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.ValidationAttributes
{
	public class DateAfterNow: ValidationAttribute
	{

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{



            if ((DateTime?)value <= DateTime.Now)
			{
				return new ValidationResult("The date must be after the current date and time.");
			}

			return ValidationResult.Success;
		}
	}
}
