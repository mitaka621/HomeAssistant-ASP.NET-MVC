using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class UserActivityLog
	{

		[MaxLength(10)]
        public string RequestType { get; set; } = string.Empty;

		[MaxLength(1000)]
		public string RequestUrl { get; set; } = string.Empty;

		public string QueryString { get; set; } = string.Empty;

		public string ActionArgumentsJson { get; set; } = string.Empty;

		[Required]
		public DateTime DateTime { get; set; }

        [Required]
		public string UserId { get; set; } = string.Empty;
		[ForeignKey(nameof(UserId))]
		public HomeAssistantUser User { get; set; } = null!;

	}
}
