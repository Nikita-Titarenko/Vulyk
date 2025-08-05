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
    public class ChatService : IChatService
    {
        private const int lastMessageLength = 26;

        private readonly ApplicationDbContext _context;

        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(CreateChatResult, int?)> GetOrCreateChatAsync(string userId, string userToAddId)
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

        public async Task<int?> GetChatAsync(string userId, string userToAddId)
        {
            return await _context.Chat
                .Where(c =>
                c.UserChats.Any(uc => uc.UserId == userId) &&
                c.UserChats.Any(uc => uc.UserId == userToAddId))
                .Select(c => (int?)c.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<ChatListDto> GetChatsAsync(string userId)
        {
            return new ChatListDto
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
                   Name = uc.Partner.FullName ?? string.Empty,
                   LastMessageText = uc.LastMessage != null ? uc.LastMessage.Text.Substring(0, lastMessageLength) : string.Empty,
                   LastMessageDateTime = uc.LastMessage != null ? uc.LastMessage.CreationDateTime : null

               }).ToListAsync()
            };
        }

        public enum CreateChatResult
        {
            Success, NotFound, CanNotAddYourself
        }
    }
}
