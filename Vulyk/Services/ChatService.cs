using System;
using Microsoft.EntityFrameworkCore;
using Vulyk.Data;
using Vulyk.Models;

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

        public enum CreateChatResult
        {
            Success, NotFound, CanNotAddYourself
        }
    }
}
