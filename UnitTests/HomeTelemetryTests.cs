using HomeAssistant.Core.Services;
using Moq.Protected;
using Moq;
using System.Net;
using Microsoft.Extensions.Configuration;
using HomeAssistant.Core.Contracts;
using Microsoft.Extensions.Logging;
using HomeAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace UnitTests
{
	[TestFixture]
	public class HomeTelemetryTests
	{
		private HomeAssistantDbContext _dbContext;
		private IHomeTelemetryService _homeTelemetryService;
		private Mock<HttpMessageHandler> _mockHttpMessageHandler;
		private HttpClient _httpClient;
		private Mock<IConfiguration> _mockConfiguration;
		private Mock<ILogger<IHomeTelemetryService>> _logger;
		private INotificationService _notificationService;

		[SetUp]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<HomeAssistantDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			_dbContext = new HomeAssistantDbContext(options);

			_mockHttpMessageHandler = new Mock<HttpMessageHandler>();
			_mockHttpMessageHandler
				.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });


			_httpClient = new HttpClient(_mockHttpMessageHandler.Object);

			_logger=new Mock<ILogger<IHomeTelemetryService>>();

			_notificationService =new NotificationService(_dbContext, new Mock<IUserService>().Object, new Mock<IimageService>().Object);

			

			_mockConfiguration = new Mock<IConfiguration>();

			_mockConfiguration.Setup(x => x.GetSection("HomeTelemetryServerIp").Value).Returns("apikey");

			_homeTelemetryService = new HomeTelemetryService(
				_mockConfiguration.Object,
				_httpClient,
				_dbContext,
				_logger.Object,
				_notificationService);
		}

		[Test]
		public async Task GetData_Returns_HomeTelemetry_Gathered_Data()
		{
			string mockData = "mocked location data";

			_mockHttpMessageHandler
				.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(mockData)
				});

			string result = await _homeTelemetryService.GetData();

			Assert.AreEqual(mockData, result);
		}

		[Test]
		public async Task SaveData_SavesDataToDatabase()
		{
			var testData = "{\"CPM\": 100, \"Temperature\": 25, \"Humidity\": 50, \"Radiation\": 2}";

			  
			await _homeTelemetryService.SaveData(testData);

			 
			var savedData = await _dbContext.homeTelemetries.FirstOrDefaultAsync();
			Assert.IsNotNull(savedData);

		}

		[Test]
		public async Task CreateNotificationIfDataIsAbnormal_DoesntSendsNotification()
		{
			  
		var testData = "{\"CPM\": 100, \"Temperature\": 25, \"Humidity\": 50, \"Radiation\": 0.5}";

			  
			var result = await _homeTelemetryService.CreateNotificationIfDataIsAbnormal(testData);

			 
			Assert.AreEqual(0, await _dbContext.Notifications.CountAsync());
			Assert.AreEqual(-1, result);

		}

		[Test]
		public async Task CreateNotificationIfDataIsAbnormal_SendsNotification()
		{

			var testData = "{\"CPM\": 100, \"Temperature\": 25, \"Humidity\": 50, \"Radiation\": 100}";

			  
			var result = await _homeTelemetryService.CreateNotificationIfDataIsAbnormal(testData);

			Assert.AreNotEqual(-1, result);
			Assert.AreEqual(1, await _dbContext.Notifications.CountAsync());

			Assert
				.AreEqual("Abnormal radiation levels detected", (await _dbContext.Notifications.FirstAsync(x => x.Id == result)).Title);
		}


	}
}
