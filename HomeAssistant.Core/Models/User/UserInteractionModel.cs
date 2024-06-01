using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models.User
{
	public class UserInteractionViewModel
	{
		public string UserId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public DateTime DateTime { get; set; }

        public string RequestType { get; set; } = string.Empty;

        public string RequestUrl { get; set; } = string.Empty;

        public string QueryString { get; set; } = string.Empty;

        public string ActionArgumentsJson { get; set; } = string.Empty;
    }
}
