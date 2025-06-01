namespace HomeAssistant.Core.Models.Notification
{
    public class NotificationsWithPfpModel
    {
        public IEnumerable<NotificationViewModel> NotificationsContent { get; set; } = new List<NotificationViewModel>();
    }
}
