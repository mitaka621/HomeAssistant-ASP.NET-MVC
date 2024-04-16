using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Infrastructure.Data.Models
{
	public class HomeTelemetry
	{
        [Key]
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public double Humidity { get; set; }

		public double Temperature { get; set; }

		public double CPM { get; set; }

		public double Radiation { get; set; }
	}
}
