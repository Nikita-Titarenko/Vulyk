using Vulyk.DTOs;

namespace Vulyk.Services
{
    public interface IMessageService
    {
        Task<int> CreateOrAddMessageToChatAsync(string userId, string text, string userToAddId);
        Task<MessageListDto> GetMessagesAsync(int chatId, string userId, string partnerId);
    }
}