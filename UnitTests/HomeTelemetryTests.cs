using System.Net;
using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Services;
using HomeAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

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
        private Mock<ILogger<INotificationService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<INotificationService>>();

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

            _logger = new Mock<ILogger<IHomeTelemetryService>>();

            _notificationService = new NotificationService(_dbContext, new Mock<IUserService>().Object, new Mock<IImageService>().Object, _mockConfiguration.Object, _mockLogger.Object);

            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(x => x.GetSection("HomeTelemetryServerIp").Value).Returns("apikey");

            _homeTelemetryService = new HomeTelemetryService(
                _mockConfiguration.Object,
                _httpClient,
                _dbContext,
                _logger.Object,
                _notificationService);
        }

        [TearDown]
        public void Dispose()
        {
            _dbContext.Dispose();
            _httpClient.Dispose();
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

            string result = await _homeTelemetryService.GetLiveData();

            Assert.That(result, Is.EqualTo(mockData));
        }

        [Test]
        public async Task SaveData_SavesDataToDatabase()
        {
            var testData = "{\"CPM\": 100, \"Temperature\": 25, \"Humidity\": 50, \"Radiation\": 2}";

            await _homeTelemetryService.SaveData(testData);

            var savedData = await _dbContext.homeTelemetries.FirstOrDefaultAsync();
            Assert.That(savedData, Is.Not.Null);
        }

        [Test]
        public async Task CreateNotificationIfDataIsAbnormal_DoesntSendsNotification()
        {
            var testData = "{\"CPM\": 100, \"Temperature\": 25, \"Humidity\": 50, \"Radiation\": 0.5}";

            var result = await _homeTelemetryService.CreateNotificationIfDataIsAbnormal(testData);

            Assert.That(await _dbContext.Notifications.CountAsync(), Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public async Task CreateNotificationIfDataIsAbnormal_SendsNotification()
        {
            var testData = "{\"CPM\": 100, \"Temperature\": 25, \"Humidity\": 50, \"Radiation\": 100}";

            var result = await _homeTelemetryService.CreateNotificationIfDataIsAbnormal(testData);

            Assert.That(result, Is.Not.EqualTo(-1));
            Assert.That(await _dbContext.Notifications.CountAsync(), Is.EqualTo(1));
            Assert.That((await _dbContext.Notifications.FirstAsync(x => x.Id == result)).Title, Is.EqualTo("Abnormal radiation levels detected"));
        }
    }
}
