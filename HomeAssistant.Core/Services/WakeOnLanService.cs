using HomeAssistant.Core.Contracts;
using HomeAssistant.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Services
{
	public class WakeOnLanService : IWakeOnLanService
	{
		private readonly ILogger _logger;
		private readonly Dictionary<string, string> pcMacPairs = new();
		public WakeOnLanService(IConfiguration configuration, ILogger<IWakeOnLanService> logger)
		{

			if (configuration.GetSection("WakeOnLanPcs") != null)
			{
                foreach (var pc in configuration.GetSection("WakeOnLanPcs").GetChildren())
                {
					try
					{
						pcMacPairs[pc["Name"]!] = pc["MAC"]!;

					} catch (Exception)
					{
						continue;
					}
                }               
			}
			
			_logger = logger;
		}

		public async Task<bool> WakeOnLan(string pcName)
		{
            if (!pcMacPairs.Keys.Contains(pcName))
            {
				_logger.LogWarning($"Could not find a pc with name {pcName} with a registered mac address");
				return false;
            }

            string macAddress = pcMacPairs[pcName];

			macAddress = macAddress.Replace("-", "").Replace(":", "").Replace(".", "").ToUpper();

			byte[] magicPacket = new byte[102];
			for (int i = 0; i < 6; i++)
				magicPacket[i] = 0xFF;
			for (int i = 6; i < 102; i += 6)
			{
				for (int j = 0; j < 6; j++)
					magicPacket[i + j] = byte.Parse(macAddress.Substring(j * 2, 2), System.Globalization.NumberStyles.HexNumber);
			}

			using (var client = new UdpClient())
			{
				client.Connect(IPAddress.Broadcast, 9);
				await client.SendAsync(magicPacket, magicPacket.Length);
			}

			_logger.LogInformation($"Sent a wake up magicPacket to {pcName}:{macAddress}");
			return true;
		}
	}
}
