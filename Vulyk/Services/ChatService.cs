using System.Data;
using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Vulyk.Data;
using Vulyk.DTOs;

namespace Vulyk.Services
{
    public class ChatService : IChatService
    {
        private const int lastMessageLength = 26;

        private readonly ApplicationDbContext _context;

        private readonly IMapper _mapper;

        private readonly IUserService _userService;

        private readonly ILogger<ChatService> _logger;

        public ChatService(ApplicationDbContext context, IUserService userService, IMapper mapper, ILogger<ChatService> logger)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
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
            if (getChatResult.Value.ChatId != null)
            {
                var createChatResult = _mapper.Map<CreateUserChatResultDto>(getChatResult.Value);
                return Result.Ok(createChatResult);
            }

                using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Chat chat = new Chat();
                _context.Chat.Add(chat);
                await _context.SaveChangesAsync();
                UserChat firstUserChat = new UserChat()
                {
                    ChatId = chat.Id,
                    UserId = userId,
                };
                UserChat secondUserChat = new UserChat()
                {
                    ChatId = chat.Id,
                    UserId = userToAddId,
                };
                _context.UserChat.Add(firstUserChat);
                _context.UserChat.Add(secondUserChat);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Result.Ok(new CreateUserChatResultDto { ChatId = chat.Id });
            }
            catch
            {
                await transaction.RollbackAsync();
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
            int? chatId = await _context.Chat
                .Where(c =>
                c.UserChats.Any(uc => uc.UserId == userId) &&
                c.UserChats.Any(uc => uc.UserId == userToAddId))
                .Select(c => (int?)c.Id)
                .FirstOrDefaultAsync();

            return Result.Ok(new GetUserChatResultDto { UserId = userToAddId, ChatId = chatId });
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
                ChatListDto chatList = new ChatListDto
                {
                    ChatItems = await _context.UserChat
    .Where(uc => uc.UserId == userId)
    .Select(uc => new
    {
        uc.ChatId,

        Partner = _context.UserChat
        .Where(x => x.ChatId == uc.ChatId && x.UserId != userId && x.ApplicationUser.EmailConfirmed)
        .Select(uc => new
        {
            Id = uc.UserId,
            uc.ApplicationUser.FullName,
            uc.ApplicationUser.LastOnline
        }).FirstOrDefault(),

        LastMessage = _context.Message
         .Where(m => m.ChatId == uc.ChatId)
         .Select(m => new { m.Text, m.CreationDateTime }).OrderByDescending(m => m.CreationDateTime).FirstOrDefault()
    })
    .Where(uc => uc.Partner != null)
    .OrderByDescending(uc => uc.LastMessage != null ? uc.LastMessage.CreationDateTime : DateTime.MinValue)
    .Select(uc => new ChatListItemDto
    {
        ChatId = uc.ChatId,
        UserId = uc.Partner!.Id,
        FullName = uc.Partner.FullName ?? string.Empty,
        LastMessageText = uc.LastMessage != null ? uc.LastMessage.Text.Substring(0, lastMessageLength) : string.Empty,
        LastMessageDateTime = uc.LastMessage != null ? uc.LastMessage.CreationDateTime : null

    }).ToListAsync()
                };
                return Result.Ok(chatList);
            }
            catch
            {
                _logger.LogError("Failed to receive chats for UserId={userId}", userId);
                return Result.Fail(new Error("Error chats receiving").WithMetadata("Code", "GetChatsError"));
            }
        }
    }
}
