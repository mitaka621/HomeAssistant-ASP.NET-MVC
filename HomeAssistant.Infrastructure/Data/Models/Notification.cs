using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [MaxLength(Constants.NameMaxLenght)]
        [Comment("Notification title")]
        public string Title { get; set; }=string.Empty;

        [MaxLength(Constants.DescriptionMaxLength)]
        [Comment("Notification description (optional)")]
        public string? Description { get; set; }

        [Comment("Date and time when the notification was created")]
        public DateTime CreatedOn { get; set; }

        public IEnumerable<NotificationUser> NotificationsUsers { get; set; } = null!;

    }
}
