using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Services;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace UnitTests
{
    [TestFixture]
    public class MessageServiceTests
    {
        private HomeAssistantDbContext _dbContext;
        private Mock<IImageService> _mockImageService;
        private IMessageService _messageService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HomeAssistantDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new HomeAssistantDbContext(options);
            _mockImageService = new Mock<IImageService>();

            _mockImageService.Setup(x => x.GetPFP(It.IsAny<string>()))
            .ReturnsAsync(new byte[0]);

            _mockImageService.Setup(x => x.GetPfpRange(It.IsAny<string>())).ReturnsAsync(new Dictionary<string, byte[]>()
            {
                {"user1",new byte[0]  }
            });

            _messageService = new MessageService(_dbContext, _mockImageService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }


        [Test]
        public async Task GetChatDetails_Returns_Chat_Details_Correctly()
        {
            var currentUser = "currentUser";
            var userId2 = "userId2";

            _dbContext.Users
                .AddRange(new HomeAssistantUser[] {
                    new HomeAssistantUser() { Id = currentUser },
                    new HomeAssistantUser() { Id = userId2 }
                });
            await _dbContext.SaveChangesAsync();

            var expectedMessageCount = 10;

            var chatRoom = new ChatRoom { User1Id = currentUser, User2Id = userId2 };
            _dbContext.ChatRooms.Add(chatRoom);
            await _dbContext.SaveChangesAsync();

            List<Message> messages = new();
            for (int i = 0; i < expectedMessageCount; i++)
            {
                messages.Add(new Message { MessageId = i, ChatRoomId = chatRoom.Id, UserId = currentUser, MessageContent = $"Message {i}", CreatedOn = DateTime.Now });
            }
            _dbContext.Messages.AddRange(messages);

            await _dbContext.SaveChangesAsync();

            var result = await _messageService.GetChatDetails(currentUser, userId2, expectedMessageCount);

            Assert.That(expectedMessageCount == result.Messages.Count());
            Assert.That("Message 9" == result.Messages.OrderByDescending(x => x.CreatedOn).First().MessageContent);

        }

        [Test]
        public async Task SendMessage_Adds_Message_Correctly()
        {

            var chatRoom = new ChatRoom { Id = 1, User1Id = "user1", User2Id = "user2" };
            _dbContext.ChatRooms.Add(chatRoom);
            await _dbContext.SaveChangesAsync();


            await _messageService.SendMessage(chatRoom.Id, "user1", "user2", "Test message");


            var message = await _dbContext.Messages.FirstOrDefaultAsync();
            Assert.That(message, Is.Not.Null);
            Assert.That("Test message" == message.MessageContent);

        }

        [Test]
        public async Task LoadMessagesRange_Returns_Messages_Correctly()
        {

            var chatRoomId = 1;
            var skip = 0;
            var take = 10;

            for (int i = 0; i < 20; i++)
            {
                _dbContext.Messages.Add(new Message { MessageId = i, ChatRoomId = chatRoomId, UserId = "user1", MessageContent = $"Message {i}", CreatedOn = DateTime.Now });
            }
            await _dbContext.SaveChangesAsync();


            var result = await _messageService.LoadMessagesRange(chatRoomId, skip, take);

            Assert.That(take == result.Count());

            Assert.That("Message 19" == result.First().MessageContent);
        }

        [Test]
        public async Task CreateChatRoom_Creates_Chat_Room_Correctly()
        {

            var userId1 = "user1";
            var userId2 = "user2";

            _dbContext.Users
                .AddRange(new HomeAssistantUser[] {
                    new HomeAssistantUser() { Id = userId1 },
                    new HomeAssistantUser() { Id = userId2 }
                });
            await _dbContext.SaveChangesAsync();


            var chatRoomId = (await _messageService.GetChatDetails(userId1, userId2)).ChatRoomId;

            Assert.That(userId1 == _dbContext.ChatRooms.First(x => x.Id == chatRoomId).User1Id);
            Assert.That(userId2 == _dbContext.ChatRooms.First(x => x.Id == chatRoomId).User2Id);
        }
    }
}
