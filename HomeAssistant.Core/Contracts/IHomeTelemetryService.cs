using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Contracts
{
	public interface IHomeTelemetryService
	{
		Task<string> GetData();

		Task SaveData(string data);
    }
}
