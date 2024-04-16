using HomeAssistant.Core.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;

namespace UnitTests
{
	[TestFixture]
	public class WeatherServiceTests
	{
		private WeatherService _weatherService;
		private Mock<HttpMessageHandler> _mockHttpMessageHandler;
		private HttpClient _httpClient;
		private Mock<IConfiguration> _mockConfiguration;

		[SetUp]
		public void Setup()
		{
			_mockHttpMessageHandler = new Mock<HttpMessageHandler>();
			_mockHttpMessageHandler
				.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });


			_httpClient = new HttpClient(_mockHttpMessageHandler.Object);

			_mockConfiguration = new Mock<IConfiguration>();

			_mockConfiguration.Setup(x => x.GetSection("ExternalServiceApiKeys")["weatherApi"]).Returns("apikey");

			_weatherService = new WeatherService(_httpClient, _mockConfiguration.Object);
		}

		[Test]
		public async Task GetWeatherJsonString_Returns_WeatherData()
		{
			double lat = 1;
			double lon = 2;
			string expectedLocationData = "mocked location data";

			_mockHttpMessageHandler
				.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(expectedLocationData)
				});

			string result = await _weatherService.GetWeatherJsonString(lat, lon);

			Assert.AreEqual(expectedLocationData, result);
		}

		[Test]
		public async Task GetLocationJsonString_Returns_LocationData()
		{
			double lat = 1;
			double lon = 2;
			string expectedLocationData = "mocked location data2";

			_mockHttpMessageHandler
				.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(expectedLocationData)
				});

			string result = await _weatherService.GetLocationJsonString(lat, lon);

			Assert.AreEqual(expectedLocationData, result);
		}
	}
}
