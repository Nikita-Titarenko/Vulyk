using System.ComponentModel;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Hubs;
using Vulyk.Models;

namespace Vulyk.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IChatService _chatService;

        public MessageService(ApplicationDbContext context, IChatService chatService)
        {
            _context = context;
            _chatService = chatService;
        }

        public async Task<MessageListDto> GetMessagesAsync(int chatId, string userId, string partnerId)
        {
            string? userName = await _context.ApplicationUser.Where(u => u.Id == partnerId).Select(u => u.FullName).FirstOrDefaultAsync();
            if (userName == null)
            {
                return new MessageListDto { };
            }
            List<MessageListItemDto> messages = await _context.Message
                .Where(m => m.ChatId == chatId)
                .Select(m => new MessageListItemDto
                {
                    IsMine = m.UserId == userId,
                    Text = m.Text,
                    CreationDateTime = m.CreationDateTime,

                }).OrderBy(m => m.CreationDateTime).ToListAsync();
            return new MessageListDto
            {
                UserId = partnerId,
                ChatId = chatId,
                UserName = userName,
                Messages = messages
            };
        }

        public async Task<int> CreateOrAddMessageToChatAsync(string userId, string text, string userToAddId)
        {
            var (result, chatIdNullable) = await _chatService.GetOrCreateChatAsync(userId, userToAddId);

            if (result == ChatService.CreateChatResult.Success)
            {
                int chatId = chatIdNullable!.Value;
                _context.Message.Add(new Message
                {
                    UserId = userId,
                    ChatId = chatId,
                    Text = text,
                    CreationDateTime = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }
            return chatIdNullable!.Value;
        }
    }
}
