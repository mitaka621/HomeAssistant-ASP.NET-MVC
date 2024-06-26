﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeAssistant.Infrastructure.Data.Models
{
	[Comment("Which user has seen what notification")]
    public class NotificationUser
    {
        [Comment("User Identifier")]
        public string UserId { get; set; } =string.Empty;

        [ForeignKey(nameof(UserId))]
        public HomeAssistantUser User { get; set; } = null!;

        [Comment("Notification Identifier")]
        public int NotificationId { get; set; }
        [ForeignKey(nameof(NotificationId))]
        public Notification Notification { get; set; } = null!;

		[Comment("Is notification dismissed by a certain user")]
		public bool IsDismissed { get; set; }
    }
}
