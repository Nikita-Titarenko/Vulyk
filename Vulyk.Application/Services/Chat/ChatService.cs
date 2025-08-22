using System.Data;
using AutoMapper;
using FluentResults;
using Microsoft.Extensions.Logging;
using Vulyk.Application.DTOs.Chat;
using Vulyk.Application.Repositories;
using Vulyk.Application.Services.User;
using Vulyk.Domain.Models;

namespace Vulyk.Application.Services.Chat
{
    public class ChatService : IChatService
    {
        private const int lastMessageLength = 26;

        private readonly IMapper _mapper;

        private readonly IUserService _userService;

        private readonly ILogger<ChatService> _logger;

        private readonly IChatRepository _chatRepository;

        public ChatService(IUserService userService, IMapper mapper, ILogger<ChatService> logger, IChatRepository chatRepository)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
            _chatRepository = chatRepository;
        }

        ///  <summary>
        ///  Create chat between two users if it doesn't exist
        ///  </summary>
        ///  <param name="userId">Identifier of the user that create chat</param>
        ///  <param name="email">Email of the adding user</param>
        ///  <returns>
        ///  <see cref="CreateUserChatResultDto"/> containing:
        ///  <list type="bullet">
        ///  <item>ChatId and UserId (partner) if operation successful</item>
        ///  <item>Error information if users not found</item>
        /// </list>
        /// </returns>
        public async Task<Result<CreateUserChatResultDto>> CreateUserChatByEmailAsync(string userId, string email)
        {
            var findUserResult = await _userService.FindUserByEmailAsync(email);
            if (!findUserResult.IsSuccess)
            {
                return Result.Fail(findUserResult.Errors);
            }

            return await CreateUserChatAsync(userId, findUserResult.Value.UserId);
        }

        ///  <summary>
        ///  Create chat between two users if it doesn't exist
        ///  </summary>
        ///  <param name="userId">Identifier of the user that create chat</param>
        ///  <param name="userToAddId">Identifier of the adding user</param>
        ///  <returns>
        ///  <see cref="CreateUserChatResultDto"/> containing:
        ///  <list type="bullet">
        ///  <item>ChatId and UserId (partner) if operation successful</item>
        ///  <item>Error information if users not found</item>
        /// </list>
        /// </returns>
        public async Task<Result<CreateUserChatResultDto>> CreateUserChatAsync(string userId, string userToAddId)
        {
            var getChatResult = await GetUserChatAsync(userId, userToAddId);
            if (getChatResult.Value.ChatId.HasValue)
            {
                var createChatResult = _mapper.Map<CreateUserChatResultDto>(getChatResult.Value);
                return Result.Ok(createChatResult);
            }
            try
            {
                var chat = await _chatRepository.CreateUserChatAsync(userId, userToAddId);
                return Result.Ok(new CreateUserChatResultDto
                {
                    ChatId = chat.Id,
                    UserId = userToAddId
                });
            }
            catch
            {
                _logger.LogError("Failed to create Chat with UserId={userId} and PartnerId={userToAddId}", userId, userToAddId);
                return Result.Fail(new Error("Invalid request").WithMetadata("Code", "InvalidRequest"));
            }
        }

        ///  <summary>
        ///  Get UserChat by email
        ///  </summary>
        ///  <param name="userId">Identifier of the user</param>
        ///  <param name="email">Email of the adding user</param>
        ///  <returns>
        ///  <see cref="CreateUserChatResultDto"/> containing:
        ///  <list type="bullet">
        ///  <item>ChatId and UserId (partner) if operation successful</item>
        ///  <item>Error information if users not found</item>
        /// </list>
        /// </returns>
        public async Task<Result<GetUserChatResultDto>> GetUserChatByEmailAsync(string userId, string email)
        {
            var findUserResult = await _userService.FindUserByEmailAsync(email);
            if (!findUserResult.IsSuccess)
            {
                return Result.Fail(findUserResult.Errors);
            }

            return await GetUserChatAsync(userId, findUserResult.Value.UserId);
        }

        ///  <summary>
        ///  Get UserChat by email
        ///  </summary>
        ///  <param name="userId">Identifier of the user</param>
        ///  <param name="userToAddId">Identifier of the adding user</param>
        ///  <returns>
        ///  <see cref="CreateUserChatResultDto"/> containing:
        ///  <list type="bullet">
        ///  <item>ChatId and UserId (partner) if operation successful</item>
        ///  <item>Error information if users not found</item>
        /// </list>
        /// </returns>
        public async Task<Result<GetUserChatResultDto>> GetUserChatAsync(string userId, string userToAddId)
        {
            var userChat = await _chatRepository.GetUserChatAsync(userId, userToAddId);
            if (userChat == null)
            {
                return Result.Ok(new GetUserChatResultDto { UserId = userToAddId});
            }
            var dto = _mapper.Map<GetUserChatResultDto>(userChat);
            dto.UserId = userToAddId;
            return Result.Ok(dto);
        }

        ///  <summary>
        ///  Get user's ChatList with ChatListItems
        ///  </summary>
        ///  <param name="userId">Identifier of the user</param>
        ///  <returns>
        ///  <see cref="ChatListDto"/> containing ChatListItems that containing:
        ///  <list type="bullet">
        ///  <item>ChatId, UserId (partner), FullName (partner), LastMessageDateTime, LastMessageText if operation successful</item>
        ///  <item>Error information if users not found</item>
        /// </list>
        /// </returns>
        public async Task<Result<ChatListDto>> GetChatsAsync(string userId)
        {
            try
            {
                return Result.Ok(await _chatRepository.GetChatsAsync(userId, lastMessageLength));
            }
            catch
            {
                _logger.LogError("Failed to receive chats for UserId={userId}", userId);
                return Result.Fail(new Error("Error chats receiving").WithMetadata("Code", "GetChatsError"));
            }
        }
    }
}
