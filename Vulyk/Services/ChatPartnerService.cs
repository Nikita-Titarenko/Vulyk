using Microsoft.EntityFrameworkCore;
using Vulyk.Data;
using Vulyk.DTOs;

namespace Vulyk.Services
{
    public class ChatPartnerService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public ChatPartnerService(IDbContextFactory<ApplicationDbContext> context)
        {
            _contextFactory = context;
        }

        internal async Task<ChatPartnerDto?> GetChatPartnerAsync(int userId, int chatId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserChat
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
