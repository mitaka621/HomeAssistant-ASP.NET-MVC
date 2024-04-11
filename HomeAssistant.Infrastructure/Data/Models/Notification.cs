using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string? InvokedBy { get; set; }
        [ForeignKey(nameof(InvokedBy))]
        public HomeAssistantUser? User { get; set; }

        public string InvokerURL { get; set; }= string.Empty;

        public IEnumerable<NotificationUser> NotificationsUsers { get; set; } = new List<NotificationUser>();

    }
}
