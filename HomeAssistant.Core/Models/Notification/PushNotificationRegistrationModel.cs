using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models.Notification
{
	public class PushNotificationRegistrationModel
	{
		[JsonPropertyName("endpoint")]
		public string Endpoint { get; set; }
		
		[JsonPropertyName("expirationTime")]
		public object? ExpirationTime { get; set; }  // Use 'object' type if the field can be null or another type

		[JsonPropertyName("keys")]
		public SubscriptionKeys Keys { get; set; }		
	}

	public class SubscriptionKeys
	{
		[JsonPropertyName("auth")]
		public string Auth { get; set; }

		[JsonPropertyName("p256dh")]
		public string P256dh { get; set; }
	}
}
