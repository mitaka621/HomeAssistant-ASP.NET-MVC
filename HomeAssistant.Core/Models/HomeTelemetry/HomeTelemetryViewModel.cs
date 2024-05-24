using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Models.HomeTelemetry
{
	public class HomeTelemetryViewModel
	{
		public DateTime DateTime { get; set; }

		public double Humidity { get; set; }

		public double Temperature { get; set; }

		public double CPM { get; set; }

		public double Radiation { get; set; }
	}
}
