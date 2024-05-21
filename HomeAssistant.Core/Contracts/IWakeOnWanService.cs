using HomeAssistant.Core.Models.WakeOnLanModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Contracts
{
	public interface IWakeOnLanService
	{
		Task<bool> WakeOnLan(string pcName);

		IEnumerable<PcModel> GetAllAvailableWakeOnLanPcs();

		Task<bool> PingHost(string ipAddress);

	}
}
