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
		public string Endpoint { get; set; }=string.Empty;
		
		[JsonPropertyName("expirationTime")]
		public object? ExpirationTime { get; set; }

		[JsonPropertyName("keys")]
		public SubscriptionKeys Keys { get; set; } = null!;

		[JsonPropertyName("deviceType")]
		public string DeviceType { get; set; } = string.Empty;
	}

	public class SubscriptionKeys
	{
		[JsonPropertyName("auth")]
		public string Auth { get; set; } = string.Empty;

		[JsonPropertyName("p256dh")]
		public string P256dh { get; set; } = string.Empty;
	}
}
