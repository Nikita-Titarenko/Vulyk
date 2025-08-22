using Vulyk.Application.DTOs.Chat;
using Vulyk.Domain.Models;

namespace Vulyk.Application.Repositories
{
    public interface IChatRepository
    {
        Task<Chat> CreateUserChatAsync(string userId, string userToAddId);
        Task<ChatListDto> GetChatsAsync(string userId, int lastMessageLength);
        Task<Chat?> GetUserChatAsync(string userId, string userToAddId);
    }
}