using System;
using System.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Hubs;
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

        public async Task<(CreateChatResult, int?)> GetOrCreateChatAsync(int userId, int userToAddId)
        {
            int? existingChatId = await GetChatAsync(userId, userToAddId);
            if (existingChatId != null)
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
                    UserId = userToAddId,
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

        public async Task<int?> GetChatAsync(int userId, int userToAddId)
        {
            return await _context.Chat
                .Where(c =>
                c.UserChats.Any(uc => uc.UserId == userId) &&
                c.UserChats.Any(uc => uc.UserId == userToAddId))
                .Select(c => (int?)c.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ChatListItemDto>> GetChatsAsync(int userId)
        {
            return await _context.UserChat
               .Where(uc => uc.UserId == userId)
               .Select(uc => new
               {
                   uc.ChatId,

                   Partner = _context.UserChat
                   .Where(x => x.ChatId == uc.ChatId && x.UserId != userId)
                   .Select(uc => new
                   {
                       Id = uc.UserId,
                       uc.User.Name,
                       uc.User.LastOnline
                   }).FirstOrDefault(),

                    LastMessage = _context.Message
                    .Where(m => m.ChatId == uc.ChatId)
                    .Select(m => new { m.Text, m.CreationDateTime }).OrderByDescending(m => m.CreationDateTime).FirstOrDefault()
               })
               .OrderByDescending(uc => uc.LastMessage != null ? uc.LastMessage.CreationDateTime : DateTime.MinValue)
               .Select(uc => new ChatListItemDto
               {
                   ChatId = uc.ChatId,
                   UserId = uc.Partner != null ? uc.Partner.Id : 0,
                   Name = uc.Partner != null ? uc.Partner.Name : string.Empty,
                   LastMessageText = uc.LastMessage != null ? uc.LastMessage.Text : string.Empty,
                   LastMessageDateTime = uc.LastMessage != null ? uc.LastMessage.CreationDateTime : null
                   
               }).Where(uc => uc.UserId != 0).ToListAsync();
        }

        public enum CreateChatResult
        {
            Success, NotFound, CanNotAddYourself
        }
    }
}
