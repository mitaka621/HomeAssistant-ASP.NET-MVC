using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeAssistant.Infrastructure.Data.Models
{
	[Comment("Notification")]
    public class Notification
    {
        [Key]
        [Comment("Notification Identifier")]
        public int Id { get; set; }

        [Required]
        [Comment("Notification title")]
        public string Title { get; set; }=string.Empty;

        [Comment("Notification description")]
        public string Description { get; set; }=string.Empty;

        [Comment("Date and time when the notification was created")]
        public DateTime CreatedOn { get; set; }

        [Comment("The user who caused the notification to be generated (could be null)")]
        public string? InvokedBy { get; set; }
        [ForeignKey(nameof(InvokedBy))]
        public HomeAssistantUser? User { get; set; }

        [Comment("The system controller and route from where the create notification was called")]
        public string InvokerURL { get; set; }= string.Empty;

        public IEnumerable<NotificationUser> NotificationsUsers { get; set; } = new List<NotificationUser>();

    }
}
