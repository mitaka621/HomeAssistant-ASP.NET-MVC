using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models.Notification
{
	public class NotificationsWithPfpModel
	{
		public IEnumerable<NotificationViewModel> NotificationsContent { get; set; } = new List<NotificationViewModel>();

		public Dictionary<string, byte[]> ProfilePictures { get; set; } = new();
    }
}
