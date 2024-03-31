using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models.Recipe
{
	public class RecipesPaginationViewModel
	{
		public int PageCount { get; set; }

		public int CurrentPage { get; set; }

		public IEnumerable<RecipeDetaislViewModel> Recipes { get; set; } = new List<RecipeDetaislViewModel>();
	}
}
