using Microsoft.EntityFrameworkCore;
using Vulyk.Application.Repositories;
using Vulyk.Domain.Models;
using Vulyk.Infrastructure.Data;

namespace Vulyk.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(string userId, string partnerId, int chatId)
        {
            return await _context.Message
                .Where(m => m.ChatId == chatId)
                .AsNoTracking()
                .OrderBy(m => m.CreationDateTime)
                .ToListAsync();
        }

        public async Task CreateMessageAsync(string userId, int chatId, string text)
        {
            _context.Message.Add(new Message
            {
                UserId = userId,
                ChatId = chatId,
                Text = text,
                CreationDateTime = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync();
        }
    }
}