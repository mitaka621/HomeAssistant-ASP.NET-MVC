using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class Message
	{
        [Key]
        public int MessageId { get; set; }

        public int ChatRoomId { get; set; }
        [ForeignKey(nameof(ChatRoomId))]
        public ChatRoom ChatRoom { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserId))]
        public HomeAssistantUser User { get; set; } = null!;

        public string MessageContent { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; }

    }
}
