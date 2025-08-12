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

        public ChatService(ApplicationDbContext context, IUserService userService, IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<Result<CreateUserChatResultDto>> CreateUserChatByEmailAsync(string userId, string email)
        {
            var findUserResult = await _userService.FindUserByEmailAsync(email);
            if (!findUserResult.IsSuccess)
            {
                return Result.Fail(findUserResult.Errors);
            }

            return await CreateUserChatAsync(userId, findUserResult.Value.UserId);
        }

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
                return Result.Fail(new Error("Invalid request").WithMetadata("Code", "InvalidRequest"));
            }
        }

        public async Task<Result<GetUserChatResultDto>> GetUserChatByEmailAsync(string userId, string email)
        {
            var findUserResult = await _userService.FindUserByEmailAsync(email);
            if (!findUserResult.IsSuccess)
            {
                return Result.Fail(findUserResult.Errors);
            }

            return await GetUserChatAsync(userId, findUserResult.Value.UserId);
        }

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
                return Result.Fail(new Error("Error chats receiving").WithMetadata("Code", "GetChatsError"));
            }
        }
    }
}
