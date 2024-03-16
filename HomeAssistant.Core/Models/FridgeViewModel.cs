using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models
{
	public class FridgeViewModel
	{
        public int PageCount { get; set; }

        public IEnumerable<ProductViewModel> Products { get; set; } = null!;
    }
}
