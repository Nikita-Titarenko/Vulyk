using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Models;

namespace Vulyk.Services
{
    public class MessageService
    {
        private readonly ApplicationDbContext _context;
        private readonly ChatPartnerService _chatPartnerService;

        public MessageService(ApplicationDbContext context, ChatPartnerService chatPartnerService)
        {
            _context = context;
            _chatPartnerService = chatPartnerService;
        }

        public async Task<MessageListDto> GetMessagesAsync(int chatId, int userId)
        {
            var partner = await _chatPartnerService.GetChatPartnerAsync(userId, chatId);
            if (partner == null)
            {
                throw new InvalidOperationException("User not found");
            }
            MessageListDto messageListDto = new MessageListDto
            {
                ChatId = chatId,
                
                UserName = partner.Name,
                Messages = await _context.Message
                .Where(m => m.ChatId == chatId)
                .Select(m => new MessageListItemDto
                {
                    Id = m.Id,
                    UserId = m.UserId,
                    CreationDateTime = m.CreationDateTime,
                    Text = m.Text,
                    IsMine = m.UserId == userId
                }).ToListAsync()
            };
            return messageListDto;
        }

        public async Task CreateMessage(int chatId, int userId, string text)
        {
            _context.Message.Add(new Message
            {
                UserId = userId,
                ChatId = chatId,
                Text = text,
                CreationDateTime = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }
    }
}
