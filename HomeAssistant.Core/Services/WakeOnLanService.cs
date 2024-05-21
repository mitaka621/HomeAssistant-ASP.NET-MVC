using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.WakeOnLanModels;
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

		private readonly List<PcModel> pcModels = new List<PcModel>();

		public WakeOnLanService(IConfiguration configuration, ILogger<IWakeOnLanService> logger)
		{
			_logger = logger;

			if (configuration.GetSection("WakeOnLanPcs") != null)
			{
				try
				{
					foreach (var pc in configuration.GetSection("WakeOnLanPcs").GetChildren())
					{
						string[] arr = pc.Value!.Split(":");

						if (arr.Length <= 1)
						{
							continue;
						}

						if (pc.Key.Contains("host"))
						{
							pcModels.Add(new PcModel()
							{
								IsHostPc = true,
								PcName = arr[0],
								MAC = arr[1],
								PcIp = arr[2],
							});
							continue;

						}

						if (pc.Key.Contains("nas"))
						{
							pcModels.Add(new PcModel()
							{
								IsNAS = true,
								PcName = arr[0],
								MAC = arr[1],
								PcIp = arr[2],
							});
							continue;
						}

						pcModels.Add(new PcModel()
						{
							PcName = arr[0],
							MAC = arr[1],
						});
					}
				}
				catch (Exception)
				{
					_logger.LogError("Invalid WakeOnLanPcs configuration");
				}
				
			}

			
		}

		public async Task<bool> WakeOnLan(string pcName)
		{
			if (!pcModels.Select(x=>x.PcName).Contains(pcName))
			{
				_logger.LogWarning($"Could not find a pc with name {pcName} with a registered mac address");
				return false;
			}
		
			bool isSent = false ;

			string macAddress = pcModels.First(x=>x.PcName==pcName).MAC;

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

		public IEnumerable<PcModel> GetAllAvailableWakeOnLanPcs()
		{
			return pcModels;
		}

		public async Task<bool> PingHost(string ipAddress)
		{
			try
			{
				Ping pingSender = new Ping();
				PingReply reply = await pingSender.SendPingAsync(ipAddress);

				if (reply.Status == IPStatus.Success)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning($"Ping to {ipAddress} failed. Exception: {ex.Message}");
				return false;
			}
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
