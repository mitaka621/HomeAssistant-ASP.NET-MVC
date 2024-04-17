using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class HomeTelemetry
	{
		[Comment("Telemetry record identifier")]
        [Key]
        public int Id { get; set; }

		[Comment("Date and time of the record")]
		public DateTime DateTime { get; set; }

		[Comment("Detected humidity")]
		public double Humidity { get; set; }

		[Comment("Detected Temperature")]
		public double Temperature { get; set; }

		[Comment("Detected clicks per minute (beta and gama rays only)")]
		public double CPM { get; set; }

		[Comment("Calculated radiation in microsilverts")]
		public double Radiation { get; set; }
	}
}
