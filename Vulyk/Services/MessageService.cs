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

        public MessageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MessageListDto> GetMessages(int chatId)
        {
            MessageListDto messageListDto = new MessageListDto
            {
                ChatId = chatId,
                UserName = "Kek",
                Messages = await _context.Message
                .Where(m => m.ChatId == chatId)
                .Select(m => new MessageListItemDto
                {
                    Id = m.Id,
                    CreationDateTime = m.CreationDateTime,
                    Text = m.Text,
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
