using HomeAssistant.Core.Models.Message;

namespace HomeAssistant.Core.Contracts
{
	public interface IMessageService
	{
		Task<ChatDetailsViewModel> GetChatDetails(string userId1, string userId2,int messagesCount=50);

		Task SendMessage(int chatRoomId,string senderId, string recipientId, string message);

		Task<IEnumerable<MessageViewModel>> LoadMessagesRange(int chatroomId, int skip, int take);
	}
}
