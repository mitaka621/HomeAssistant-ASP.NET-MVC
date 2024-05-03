using HomeAssistant.Core.Models.NAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Contracts
{
	public interface INASService
	{
		Task<IEnumerable<DirViewModel>?> GetData(string path);

		Task<HttpResponseMessage?> GetFileString(string path);

    }
}
