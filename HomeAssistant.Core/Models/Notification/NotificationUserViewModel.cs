using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models.Notification
{
	public class NotificationUserViewModel
	{
		public string? Id { get; set; } = null!;
        public byte[] Photo { get; set; } = null!;

		public string? FirstName { get; set; } = string.Empty;


    }
}
