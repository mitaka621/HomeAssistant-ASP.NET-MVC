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
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Services
{
	public class WakeOnLanService : IWakeOnLanService
	{
		private readonly ILogger _logger;
		private readonly Dictionary<string, string> pcMacPairs = new();
		public WakeOnLanService(IConfiguration configuration, ILogger<IWakeOnLanService> logger)
		{
			_logger = logger;

			if (configuration.GetSection("WakeOnLanPcs") != null)
			{
				foreach (var pc in configuration.GetSection("WakeOnLanPcs").GetChildren())
				{
					try {
						string[] arr = pc.Value!.Split(":");
						pcMacPairs[arr[0]] = arr[1];
					}
					catch (Exception)
					{
						_logger.LogWarning("Could not parse input in wakeonLan");
					}
					
				}
			}

			
		}

		public async Task<bool> WakeOnLan(string pcName)
		{
			if (!pcMacPairs.Keys.Contains(pcName))
			{
				_logger.LogWarning($"Could not find a pc with name {pcName} with a registered mac address");
				return false;
			}
		
			bool isSent = false ;

			string macAddress = pcMacPairs[pcName];

			_logger.LogInformation($"Sending a wake package to {macAddress}");

			byte[] magicPacket = BuildMagicPacket(macAddress);

            foreach (var ip in GetLocalIPv4Addresses())
            {
				if (ip == null)
                {
					continue;
                }
                await SendWakeOnLan(ip, IPAddress.Parse("224.0.0.1"), magicPacket);
				isSent=true;
			}



            return isSent;
		}

		private byte[] BuildMagicPacket(string macAddress) // MacAddress in any standard HEX format
		{
			macAddress = Regex.Replace(macAddress, "[: -]", "");
			byte[] macBytes = Convert.FromHexString(macAddress);

			IEnumerable<byte> header = Enumerable.Repeat((byte)0xff, 6); //First 6 times 0xff
			IEnumerable<byte> data = Enumerable.Repeat(macBytes, 16).SelectMany(m => m); // then 16 times MacAddress
			return header.Concat(data).ToArray();
		}

		private async Task SendWakeOnLan(IPAddress localIpAddress, IPAddress multicastIpAddress, byte[] magicPacket)
		{
			using UdpClient client = new(new IPEndPoint(localIpAddress, 0));

			client.EnableBroadcast = true;			
			await client.SendAsync(magicPacket, magicPacket.Length, new IPEndPoint(multicastIpAddress, 9));
		}

		private IPAddress[] GetLocalIPv4Addresses()
		{
			var ipAddresses = new List<IPAddress>();

			foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (networkInterface.OperationalStatus == OperationalStatus.Up)
				{
					foreach (UnicastIPAddressInformation unicastIPAddressInformation in networkInterface.GetIPProperties().UnicastAddresses)
					{
						if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork &&
							!IPAddress.IsLoopback(unicastIPAddressInformation.Address))
						{
							ipAddresses.Add(unicastIPAddressInformation.Address);
						}
					}
				}
			}

			return ipAddresses.ToArray();
		}
	}
}
