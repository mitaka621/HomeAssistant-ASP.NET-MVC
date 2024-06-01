using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models.User
{
	public class UsersInteractionsPaginationViewModel
	{
		public List<UserInteractionViewModel> Interactions { get; set; } = new();

		public int RecordsOnPage { get; set; }

		public int CurrentPage { get; set; }

		public int TotalPages { get; set; }
	}
}
