using System;
using Microsoft.EntityFrameworkCore;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Models;
using Vulyk.ViewModels;

namespace Vulyk.Services
{
    public class ChatService
    {
        private readonly ApplicationDbContext _context;

        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        private readonly ChatPartnerService _chatPartnerService;

        private readonly UserService _userService;

        public ChatService(ApplicationDbContext context, IDbContextFactory<ApplicationDbContext> contextFactory, ChatPartnerService chatPartnerService, UserService userService)
        {
            _context = context;
            _chatPartnerService = chatPartnerService;
            _userService = userService;
            _contextFactory = contextFactory;
        }

        public async Task<(CreateChatResult, int?)> CreateChatAsync(int userId, string login, string phone, CreateType createType)
        {
            int? foundUserId = await _userService.FindUserAsync(login, phone, createType);
            if (foundUserId == null)
            {
                return (CreateChatResult.NotFound, null);
            }
            if (foundUserId == userId)
            {
                return (CreateChatResult.CanNotAddYourself, null);
            }

            int? existingChatId = await _context.Chat.Where(c =>
                            c.UserChats.Any(uc => uc.UserId == userId) &&
                            c.UserChats.Any(uc => uc.UserId == foundUserId)
            ).Select(c => c.Id).FirstOrDefaultAsync();
            if (existingChatId != 0)
            {
                return (CreateChatResult.Success, existingChatId);
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
                    UserId = foundUserId.Value,
                };
                _context.UserChat.Add(firstUserChat);
                _context.UserChat.Add(secondUserChat);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return (CreateChatResult.Success, chat.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<ChatListItemDto>> GetChatsAsync(int userId)
        {
            List<int> chatIds = await _context.UserChat
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.ChatId)
                .ToListAsync();
            var tasks = chatIds
                .Select(async cId =>
                {
                    using ApplicationDbContext localContext = _contextFactory.CreateDbContext();
                    int chatId = cId;

                    var partner = await _chatPartnerService.GetChatPartnerAsync(userId, cId);
                    if (partner == null)
                    {
                        throw new InvalidOperationException("Chat partner not found");
                    }
                    Message? lastMessage = await localContext.Message
                    .Where(m => m.ChatId == cId)
                    .OrderByDescending(m => m.CreationDateTime).FirstOrDefaultAsync();
                    return new ChatListItemDto
                    {
                        ChatId = chatId,
                        Name = partner.Name,
                        LastMessageText = lastMessage?.Text,
                        LastMessageDateTime = lastMessage?.CreationDateTime
                    };
                });
            return (await Task.WhenAll(tasks)).ToList();
        }

        public enum CreateChatResult
        {
            Success, NotFound, CanNotAddYourself
        }
    }
}
