using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models.Recipe
{
	public class RecipeDetaislViewModel
	{
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; }=null!;

        public byte[]? Photo { get; set; }

        public IEnumerable<string> Products { get; set; }=new List<string>();

        public IEnumerable<StepViewModel> Steps { get; set; } = new List<StepViewModel>();
    }
}
