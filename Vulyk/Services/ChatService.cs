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

        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(CreateChatResult, int?)> CreateChatAsync(int userId, string login, string phone, CreateType createType)
        {
            UserService userService = new UserService(_context);
            int? foundUserId = await userService.FindUserAsync(login, phone, createType);
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
            return await _context.UserChat
                .Where(uc => uc.UserId == userId)
                .Select(uc => new ChatListItemDto {
                    ChatId = uc.ChatId,

                    Name = _context.UserChat
                    .Where(x => x.ChatId == uc.ChatId && x.UserId != uc.UserId)
                    .Select(x => x.User.Name).FirstOrDefault(),

                    LastMessageText = _context.Message
                    .Where(m => m.UserId == uc.UserId && m.ChatId == uc.ChatId)
                    .OrderByDescending(m => m.CreationDateTime)
                    .Select(m => m.Text)
                    .FirstOrDefault(),

                    LastMessageDateTime = _context.Message
                    .Where(m => m.UserId == uc.UserId && m.ChatId == uc.ChatId)
                    .OrderByDescending(m => m.CreationDateTime)
                    .Select(m => m.CreationDateTime)
                    .FirstOrDefault(),
                })
                .ToListAsync();
        }

        public enum CreateChatResult
        {
            Success, NotFound, CanNotAddYourself
        }
    }
}
