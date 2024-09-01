using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.NAS;
using HomeAssistant.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HomeAssistant.Core.Services
{
	public class NASHostService : INASHostService
	{
		public static string currentHostIp = "0.0.0.0";

		private readonly HttpClient httpClient;
		private readonly ILogger _logger;

		public NASHostService(IConfiguration configuration, HttpClient _httpClient, HomeAssistantDbContext dbcontext, ILogger<IHomeTelemetryService> logger, INotificationService notificationService)
		{

			if (configuration.GetSection("ExternalServiceApiKeys")["NASHostToken"] != null)
			{
				Token = configuration.GetSection("ExternalServiceApiKeys")["NASHostToken"];
			}

			httpClient = _httpClient;
			httpClient.DefaultRequestHeaders.Add("token", Token);
			httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
			_logger = logger;
		}

		public string Token { get; set; } = string.Empty;

		public async Task<IEnumerable<DirViewModel>?> GetData(string path, int skip = 0, int take = 100)
		{
			if (!await CheckConnection())
			{
				if (!await ScanForAvailibleHost())
				{
					_logger.LogWarning("Couldnt find NAS host.");
					return null;
				}

			}
			try
			{
				var data = await httpClient
				.GetStringAsync($"http://{currentHostIp}:3000/getDir?path={path}&skip={skip}&take={take}");
				if (!String.IsNullOrEmpty(data))
				{
					return JsonSerializer.Deserialize<IEnumerable<DirViewModel>>(data);
				}
			}
			catch (Exception ex)
			{
				_logger.LogInformation(ex, $"Host {currentHostIp} unavailable. Trying to look for other host");

				return null;
			}

			return null;
		}

		public async Task<HttpResponseMessage?> GetFileString(string path)
		{
			if (!await CheckConnection())
			{
				if (!await ScanForAvailibleHost())
				{
					_logger.LogWarning("Couldnt find NAS host.");
					return null;
				}

			}
			try
			{
				httpClient.Timeout = TimeSpan.FromSeconds(10);

				var data = await httpClient
				.GetAsync($"http://{currentHostIp}:3000/getDir?path=" + path, HttpCompletionOption.ResponseHeadersRead);


				return data;
			}
			catch (Exception ex)
			{
				_logger.LogInformation(ex, $"Host {currentHostIp} unavailable. Trying to look for other host");

				return null;
			}


		}

		public async Task<HttpResponseMessage?> GetPhoto(string path, bool isFull = false)
		{
			try
			{
				httpClient.Timeout = TimeSpan.FromSeconds(10);

				if (!isFull)
				{
					return await httpClient
				   .GetAsync($"http://{currentHostIp}:3000/getphoto?path=" + path);
				}
				else
				{
					return await httpClient
				   .GetAsync($"http://{currentHostIp}:3000/getphoto?path={path}&full=true", HttpCompletionOption.ResponseHeadersRead);
				}


			}
			catch (Exception ex)
			{
				_logger.LogInformation(ex, $"Host {currentHostIp} unavailable. Trying to look for other host");

				return null;
			}

		}

		public async Task<HttpResponseMessage?> GetVideoRange(string path, string rangeHeader)
		{
			try
			{
				httpClient.Timeout = TimeSpan.FromSeconds(10);

				httpClient.DefaultRequestHeaders.Add("Range", rangeHeader);

				return await httpClient
				.GetAsync($"http://{currentHostIp}:3000/play?path={path}");
			}
			catch (Exception ex)
			{
				_logger.LogInformation(ex, $"Host {currentHostIp} unavailable. Trying to look for other host");

				return null;
			}
		}

		public async Task<DirViewModel?> GetPhotoInfo(string path)
		{
			try
			{
				var data = await httpClient
				.GetStringAsync($"http://{currentHostIp}:3000/getPhotoInfo?path={path}");
				if (!String.IsNullOrEmpty(data))
				{
					return JsonSerializer.Deserialize<DirViewModel>(data);
				}
			}
			catch (Exception ex)
			{
				_logger.LogInformation(ex, $"Host {currentHostIp} unavailable. Trying to look for other host");

				return null;
			}

			return null;
		}

		public async Task<PhotoPrevNextPaths?> GetPrevAndNextPhotoLocation(string currentPhotoPath)
		{
			try
			{
				httpClient.Timeout = TimeSpan.FromSeconds(10);

				var data = await httpClient
					 .GetStringAsync($"http://{currentHostIp}:3000/getPrevAndNextPhotoName?path=" + currentPhotoPath);

				if (!String.IsNullOrEmpty(data))
				{
					return JsonSerializer.Deserialize<PhotoPrevNextPaths>(data);
				}

				return null;
			}
			catch (Exception ex)
			{
				_logger.LogInformation(ex, $"Host {currentHostIp} unavailable. Trying to look for other host");

				return null;
			}
		}

		public async Task<bool> CheckConnection()
		{
			try
			{
				HttpClient tempclient = new();

				tempclient.Timeout = TimeSpan.FromMilliseconds(50);
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"http://{currentHostIp}:3000/");
				request.Headers.Add("token", Token);


				var response = await tempclient.SendAsync(request);
				if (response.StatusCode != System.Net.HttpStatusCode.OK)
				{
					return false;
				}

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Failed Connection checking");
				return false;
			}

		}

		public async Task<bool> ScanForAvailibleHost()
		{
			int start = 2;

			string ip = "192.168.0.";

			HttpClient tempClient = new HttpClient();

			tempClient.Timeout = TimeSpan.FromMilliseconds(50);

			List<Task<HttpResponseMessage>> tasks = new List<Task<HttpResponseMessage>>();
			for (int i = start; i < 255; i++)
			{
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"http://{ip + i}:3000/");
				request.Headers.Add("token", Token);
				try
				{
					tasks.Add(tempClient.SendAsync(request));
				}
				catch (Exception ex)
				{
					_logger.LogInformation(ex, $"Error reaching http://{ip + i}:3000/ - NAS host is not there");
				}

			}

			try
			{
				await Task.WhenAll(tasks);
			}
			catch (Exception ex)
			{
				_logger.LogInformation(ex, $"Error - NAS host is not there");
			}


			foreach (var item in tasks)
			{
				try
				{
					if (item.IsCanceled)
					{
						continue;
					}
					var tempHttpMessage = item.Result;

					if (tempHttpMessage.StatusCode == System.Net.HttpStatusCode.OK)
					{
						currentHostIp = await tempHttpMessage.Content.ReadAsStringAsync();
						return true;
					}
				}
				catch (Exception ex)
				{
					_logger.LogInformation(ex, $"Error - NAS host is not there");
				}
			}


			return false;
		}
	}
}
