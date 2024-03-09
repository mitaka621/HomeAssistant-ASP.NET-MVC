using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Contracts
{
	public interface IPFPService
	{
		Task SaveImage(string userId, byte[] imageData);

		Task<byte[]> GetImage(string userId);
	}
}
