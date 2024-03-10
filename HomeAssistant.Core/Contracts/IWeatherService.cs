using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Contracts
{
	public interface IWeatherService
	{
		Task<string> GetWeatherJsonString(double lat, double lon);

		Task<string> GetLocationJsonString(double lat, double lon);
	}
}
