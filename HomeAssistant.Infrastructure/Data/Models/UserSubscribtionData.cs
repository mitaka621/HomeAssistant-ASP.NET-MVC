using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class UserSubscribtionData
	{
        public string UserId { get; set; }=string.Empty;
		[ForeignKey(nameof(UserId))]
		public HomeAssistantUser User { get; set; } = null!;

        [MaxLength(100)]
        public string DeviceType { get; set; } = string.Empty;

		[MaxLength(2000)]
		public string? PushNotificationEndpoint { get; set; }

		[MaxLength(500)]
		public string? PushNotificationAuth { get; set; }

		[MaxLength(500)]
		public string? P256dh { get; set; }

	}
}
