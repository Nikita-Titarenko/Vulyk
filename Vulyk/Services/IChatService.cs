using FluentResults;
using Vulyk.DTOs;

namespace Vulyk.Services
{
    public interface IChatService
    {
        Task<Result<CreateUserChatResultDto>> CreateUserChatAsync(string userId, string userToAddId);
        Task<Result<CreateUserChatResultDto>> CreateUserChatByEmailAsync(string userId, string userToAddEmail);
        Task<Result<GetUserChatResultDto>> GetUserChatAsync(string userId, string userToAddId);
        Task<Result<GetUserChatResultDto>> GetUserChatByEmailAsync(string userId, string userToAddEmail);
        Task<Result<ChatListDto>> GetChatsAsync(string userId);
    }
}