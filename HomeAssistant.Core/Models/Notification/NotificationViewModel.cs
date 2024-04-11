namespace HomeAssistant.Core.Models.Notification
{
	public class NotificationViewModel
	{
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

		public string Description { get; set; }=string.Empty;

        public DateTime CreatedOn { get; set; }

		public NotificationUserViewModel Invoker { get; set; } = null!;

        public string Source { get; set; } = string.Empty;
    }
}
