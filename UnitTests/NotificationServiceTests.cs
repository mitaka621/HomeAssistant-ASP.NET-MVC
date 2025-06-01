using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models;
using HomeAssistant.Core.Services;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests
{
    [TestFixture]
    public class NotificationServiceTests
    {
        private HomeAssistantDbContext _dbContext;
        private Mock<IUserService> _mockUserService;
        private Mock<IImageService> _mockImageService;
        private INotificationService _notificationService;
        private Mock<ILogger<INotificationService>> _mockLogger;
        private Mock<IConfiguration> _mockConfiguration;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HomeAssistantDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new HomeAssistantDbContext(options);
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<INotificationService>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockImageService = new Mock<IImageService>();

            _mockImageService.Setup(x => x.GetPfpRange(It.IsAny<string>())).ReturnsAsync(new Dictionary<string, byte[]>()
            {
                {"user1",new byte[0]  }
            });

            _mockUserService.Setup(x => x.GetAllApprovedNotDeletedUsersAsync()).ReturnsAsync(new List<UserDetailsViewModel>

            {   new UserDetailsViewModel()
                {
                    Id = "user1",
                    FirstName = "misho1",
                },
                new UserDetailsViewModel()
                {
                    Id = "user2",
                    FirstName = "misho2",
                },
                new UserDetailsViewModel()
                {
                    Id = "user3",
                    FirstName = "misho3",
                },
            });

            _mockUserService.Setup(x => x.GetAllApprovedNotDeletedUsersIds()).ReturnsAsync(new List<string>
            {
                "user1",
                "user2",
                "user3"
            });

            SeedData();
            _notificationService = new NotificationService(_dbContext, _mockUserService.Object, _mockImageService.Object, _mockConfiguration.Object, _mockLogger.Object);
        }
        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        private async void SeedData()
        {

            _dbContext.Users.Add(new HomeAssistantUser()
            {
                Id = "user1",
                FirstName = "misho",
                IsDeleted = false,
            });

            _dbContext.Notifications.Add(new Notification()
            {
                Id = 100,
                CreatedOn = DateTime.Now,
                Description = "nice description",
                InvokedBy = "user1",
                InvokerURL = "/system",
                Title = "Title",

            });

            _dbContext.Notifications.Add(new Notification()
            {
                Id = 101,
                CreatedOn = DateTime.Now,
                Description = "very nice description",
                InvokedBy = null,
                InvokerURL = "/system",
                Title = "Title 2",

            });

            _dbContext.Notifications.Add(new Notification()
            {
                Id = 102,
                CreatedOn = DateTime.Now,
                Description = "very nice description",
                InvokedBy = "user1",
                InvokerURL = "/Fridge",
                Title = "Title 3",

            });

            _dbContext.NotificationsUsers.Add(new NotificationUser()
            {
                UserId = "user1",
                NotificationId = 100,

            });


            await _dbContext.SaveChangesAsync();
        }

        [Test]
        public async Task CreateNotificationForAllUsers_Creates_Notification_For_All_Users()
        {

            string title = "Test Title";
            string description = "Test Description";
            string invokerURL = "Test URL";
            string invokerId = "Test Invoker";


            var notificationId = await _notificationService.CreateNotificationForAllUsers(title, description, invokerURL, invokerId);


            var notification = await _dbContext.Notifications.FindAsync(notificationId);
            Assert.That(notification, Is.Not.Null);
            Assert.That(title == notification.Title);
            Assert.That(description == notification.Description);
        }

        [Test]
        public async Task GetNotificationsForUser_Returns_Notifications_For_User()
        {
            var notifications = await _notificationService.GetNotificationsForUser("user1");

            Assert.That("user1" == (await _dbContext.NotificationsUsers.FirstAsync(x => x.NotificationId == notifications.NotificationsContent.First().Id)).UserId);
            Assert.That("Title" == (await _dbContext.NotificationsUsers.FirstAsync(x => x.NotificationId == notifications.NotificationsContent.First().Id)).Notification.Title);
        }

        [Test]
        public async Task CreateNotificationForSpecificUser_Creates_Notification_For_Specific_User()
        {

            string title = "new Title";
            string description = "Test Description";
            string invokerURL = "/system";


            var notificationId = await _notificationService.CreateNotificationForSpecificUser(title, description, invokerURL, "user1", "user1");

            Assert.That(title == (await _dbContext.NotificationsUsers.FirstAsync(x => x.NotificationId == notificationId)).Notification.Title);
        }


        [Test]
        public async Task CreateNotificationForAllUsersExceptOne_Creates_Notifications_For_All_Users_Except_one()
        {

            string title = "new Title";
            string description = "Test Description";
            string invokerURL = "/system";

            _dbContext.Users.Add(new HomeAssistantUser()
            {
                Id = "user2",
                FirstName = "2"
            });

            _dbContext.Users.Add(new HomeAssistantUser()
            {
                Id = "user3",
                FirstName = "3"
            });

            await _dbContext.SaveChangesAsync();


            var notificationId = await _notificationService.CreateNotificationForAllUsersExceptOne(title, description, "user1", invokerURL, "user1");

            Assert.That(title == (await _dbContext.Notifications.FirstAsync(x => x.Id == notificationId)).Title);

            Assert.That(1 == (await _notificationService.GetNotificationsForUser("user1")).NotificationsContent.Count());

            Assert.That(notificationId != (await _notificationService.GetNotificationsForUser("user1")).NotificationsContent.First().Id);

            var a = await _dbContext.NotificationsUsers.Where(x => x.UserId == "user2").CountAsync();

            Assert.That(1 == a);

            Assert.That(notificationId == (await _notificationService.GetNotificationsForUser("user2")).NotificationsContent.First().Id);

        }


        [Test]
        public async Task GetNotification_Returns_Notification_For_Given_Id()
        {
            var result = await _notificationService.GetNotification(100);

            Assert.That(result, Is.Not.Null);
            Assert.That(100 == result.Id);
            Assert.That("Title" == result.Title);
        }

        [Test]
        public async Task GetTop20ProductRelatedNotification_Returns_Top_20_Product_Related_Notifications()
        {
            var result = await _notificationService.GetTop20ProductRelatedNotification();

            Assert.That(result, Is.Not.Null);
            Assert.That(1 == result.Count());
            Assert.That("Title 3" == result.First().Title);
        }

        [Test]
        public async Task DismissNotification_Dismisses_Notification_For_Given_User_And_NotificationId()
        {
            var userId = "testUserId";
            var notificationId = 100;
            var notificationUser = new NotificationUser { UserId = userId, NotificationId = notificationId };
            _dbContext.NotificationsUsers.Add(notificationUser);
            await _dbContext.SaveChangesAsync();

            await _notificationService.DismissNotification(userId, notificationId);

            var dismissedNotification = await _dbContext.NotificationsUsers.FirstOrDefaultAsync(x => x.UserId == userId && x.NotificationId == notificationId);
            Assert.That(dismissedNotification, Is.Not.Null);
            Assert.That(dismissedNotification.IsDismissed);
        }


    }
}
