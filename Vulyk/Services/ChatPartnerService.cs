using Microsoft.EntityFrameworkCore;
using Vulyk.Data;
using Vulyk.DTOs;

namespace Vulyk.Services
{
    public class ChatPartnerService
    {
        private readonly ApplicationDbContext _context;

        public ChatPartnerService(ApplicationDbContext context)
        {
            _context = context;
        }

        internal async Task<ChatPartnerDto?> GetChatPartnerAsync(int userId, int chatId)
        {
            return await _context.UserChat
                .Where(uc => uc.ChatId == chatId && uc.UserId != userId)
                .Select(uc => new ChatPartnerDto
                {
                    Id = uc.UserId,
                    Name = uc.User.Name,
                    LastOnline = uc.User.LastOnline
                }).FirstOrDefaultAsync();
        }
    }
}
