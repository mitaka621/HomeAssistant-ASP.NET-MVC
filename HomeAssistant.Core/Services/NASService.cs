using Amazon.Runtime.Internal;
using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.NAS;
using HomeAssistant.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Services
{
    public class NASService : INASService
    {
        public static string currentHostIp = "0.0.0.0";

        private readonly HttpClient httpClient;
        private readonly ILogger _logger;

        public NASService(IConfiguration configuration, HttpClient _httpClient, HomeAssistantDbContext dbcontext, ILogger<IHomeTelemetryService> logger, INotificationService notificationService)
        {

            if (configuration.GetSection("ExternalServiceApiKeys")["NASToken"] != null)
            {
                Token = configuration.GetSection("ExternalServiceApiKeys")["NASToken"];
            }

            httpClient = _httpClient;
            httpClient.DefaultRequestHeaders.Add("token", Token);
            httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
            _logger = logger;
        }

        public string Token { get; set; } = string.Empty;

        public async Task<IEnumerable<DirViewModel>?> GetData(string path,int skip=0, int take=100)
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
                .GetStringAsync($"http://{currentHostIp}:3000/getDir?path={path}&skip={skip}&take={take}" );
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

        private async Task<bool> CheckConnection()
        {
            try
            {
                HttpClient tempclient = new();

                tempclient.Timeout = TimeSpan.FromSeconds(1);
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

        private async Task<bool> ScanForAvailibleHost()
        {
            int start = 10;

            string ip = "192.168.0.";

            HttpClient tempClient = new HttpClient();

            tempClient.Timeout = TimeSpan.FromMilliseconds(50);


            for (int i = start; i < 255; i++)
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"http://{ip + i}:3000/");
                request.Headers.Add("token", Token);
                try
                {
                    var result = await tempClient.SendAsync(request);

                
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        currentHostIp = ip + i;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Error reaching http://{ip + i}:3000/ - NAS host is not there");
                }

            }





            return false;
        }
    }
}
