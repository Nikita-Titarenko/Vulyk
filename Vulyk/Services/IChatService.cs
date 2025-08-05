using Vulyk.DTOs;

namespace Vulyk.Services
{
    public interface IChatService
    {
        Task<int?> GetChatAsync(string userId, string userToAddId);
        Task<ChatListDto> GetChatsAsync(string userId);
        Task<(ChatService.CreateChatResult, int?)> GetOrCreateChatAsync(string userId, string userToAddId);
    }
}