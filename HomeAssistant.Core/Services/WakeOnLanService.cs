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

			if (configuration.GetSection("WakeOnLanPcs") != null)
			{
				foreach (var pc in configuration.GetSection("WakeOnLanPcs").GetChildren())
				{
					try
					{
						pcMacPairs[pc["Name"]!] = pc["MAC"]!;

					}
					catch (Exception)
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

			bool isSent = false ;

			string macAddress = pcMacPairs[pcName];

			byte[] magicPacket = BuildMagicPacket(macAddress);
			foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces().Where((n) =>
				n.NetworkInterfaceType != NetworkInterfaceType.Loopback && n.OperationalStatus == OperationalStatus.Up))
			{
				IPInterfaceProperties iPInterfaceProperties = networkInterface.GetIPProperties();
				foreach (MulticastIPAddressInformation multicastIPAddressInformation in iPInterfaceProperties.MulticastAddresses)
				{
					IPAddress multicastIpAddress = multicastIPAddressInformation.Address;
					if (multicastIpAddress.ToString().StartsWith("ff02::1%", StringComparison.OrdinalIgnoreCase)) // Ipv6: All hosts on LAN (with zone index)
					{
						UnicastIPAddressInformation? unicastIPAddressInformation = iPInterfaceProperties.UnicastAddresses.Where((u) =>
							u.Address.AddressFamily == AddressFamily.InterNetworkV6 && !u.Address.IsIPv6LinkLocal).FirstOrDefault();
						if (unicastIPAddressInformation != null)
						{
							await SendWakeOnLan(unicastIPAddressInformation.Address, multicastIpAddress, magicPacket);

							isSent=true;
						}
					}
					else if (multicastIpAddress.ToString().Equals("224.0.0.1")) // Ipv4: All hosts on LAN
					{
						UnicastIPAddressInformation? unicastIPAddressInformation = iPInterfaceProperties.UnicastAddresses.Where((u) =>
							u.Address.AddressFamily == AddressFamily.InterNetwork && !iPInterfaceProperties.GetIPv4Properties().IsAutomaticPrivateAddressingActive).FirstOrDefault();
						if (unicastIPAddressInformation != null)
						{
							await SendWakeOnLan(unicastIPAddressInformation.Address, multicastIpAddress, magicPacket);

							isSent = true;
						}
					}
				}
			}
			return isSent;
		}

		static byte[] BuildMagicPacket(string macAddress) // MacAddress in any standard HEX format
		{
			macAddress = Regex.Replace(macAddress, "[: -]", "");
			byte[] macBytes = Convert.FromHexString(macAddress);

			IEnumerable<byte> header = Enumerable.Repeat((byte)0xff, 6); //First 6 times 0xff
			IEnumerable<byte> data = Enumerable.Repeat(macBytes, 16).SelectMany(m => m); // then 16 times MacAddress
			return header.Concat(data).ToArray();
		}

		static async Task SendWakeOnLan(IPAddress localIpAddress, IPAddress multicastIpAddress, byte[] magicPacket)
		{
			using UdpClient client = new(new IPEndPoint(localIpAddress, 0));
			await client.SendAsync(magicPacket, magicPacket.Length, new IPEndPoint(multicastIpAddress, 9));
		}
	}
}
