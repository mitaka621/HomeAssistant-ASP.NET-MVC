using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.Message;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Services
{
	public class MessageService : IMessageService
	{
		private readonly HomeAssistantDbContext _dbContext;
		private readonly IimageService _imageService;
		public MessageService(HomeAssistantDbContext dbContext, IimageService imageService)
		{
			_dbContext = dbContext;
			_imageService = imageService;
		}

		public async Task<ChatDetailsViewModel> GetChatDetails(string currentUser, string userId2, int messagesCount = 50)
		{
            if (currentUser==userId2)
            {
				throw new InvalidOperationException();
            }

            var chatroom = await _dbContext.ChatRooms
				.Include(x => x.Messages.OrderByDescending(x=>x.CreatedOn).Take(messagesCount))
				.FirstOrDefaultAsync(x => (x.User1Id == currentUser && x.User2Id == userId2) || (x.User1Id == userId2 && x.User2Id == currentUser));

			if (chatroom == null)
			{
				chatroom = await CreateChatRoom(currentUser, userId2);
			}

			ChatDetailsViewModel model = new();

			model.Messages = chatroom.Messages.Select(x => new MessageViewModel()
			{
				CreatedOn = x.CreatedOn,
				MessageContent = x.MessageContent,
				UserId = x.UserId,
			}).OrderBy(x=>x.CreatedOn).ToList();

			model.ChatRoomId = chatroom.Id;
			model.currentUserPhoto = await _imageService.GetPFP(currentUser);
			model.currentUserId = currentUser;
			model.UserPhoto2 = await _imageService.GetPFP(userId2);
			model.Username2 = (await _dbContext.Users.FirstAsync(x => x.Id == userId2)).UserName;
			model.UserId2 = userId2;

			return model;
		}

		public async Task SendMessage(int chatRoomId, string senderId, string recipientId, string message)
		{
			var chatRoom = await _dbContext.ChatRooms
				.AsNoTracking()
				.Include(x=>x.Messages)
				.FirstOrDefaultAsync(x => x.Id == chatRoomId && ((x.User1Id == senderId && x.User2Id == recipientId) || (x.User1Id == recipientId && x.User2Id == senderId)));

			if (chatRoom == null)
			{
				throw new ArgumentNullException();
			}

			_dbContext.Messages.Add(new Message()
			{
				MessageId= chatRoom.Messages.Count()+1,
				ChatRoomId = chatRoomId,
				CreatedOn = DateTime.Now,
				MessageContent = message,
				UserId = senderId
			});

			await _dbContext.SaveChangesAsync();

		}


		public async Task<IEnumerable<MessageViewModel>> LoadMessagesRange(int chatroomId, int skip, int take)
		{
			return await _dbContext.Messages
				.AsNoTracking()
				.Where(x=>x.ChatRoomId==chatroomId)
				.OrderByDescending(x=>x.CreatedOn)
				.Skip(skip)
				.Take(take)
				.Select(x=>new MessageViewModel()
				{
					CreatedOn= x.CreatedOn,
					MessageContent = x.MessageContent,
					UserId=x.UserId,
				})
				.ToListAsync();
		}

		private async Task<ChatRoom> CreateChatRoom(string userId1, string userId2)
		{
			var user1 = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId1);

			var user2 = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId2);

			if (user1 == null || user2 == null)
			{
				throw new ArgumentNullException();
			}
			var chatRoom = new ChatRoom()
			{
				User1Id = userId1,
				User2Id = userId2,
			};
			_dbContext.ChatRooms.Add(chatRoom);

			await _dbContext.SaveChangesAsync();

			return chatRoom;
		}
	}
}
