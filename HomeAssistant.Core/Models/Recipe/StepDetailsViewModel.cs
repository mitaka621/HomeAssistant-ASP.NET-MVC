using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models.Recipe
{
	public class StepDetailsViewModel
	{
		public string Name { get; set; } = string.Empty;

		public string Description { get; set; } = string.Empty;

		public int? Duration { get; set; }

        public int StepNumber { get; set; }
    }
}
