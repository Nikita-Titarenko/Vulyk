using Microsoft.EntityFrameworkCore;
using Vulyk.Application.DTOs.Chat;
using Vulyk.Application.Repositories;
using Vulyk.Application.Services.User;
using Vulyk.Domain.Models;
using Vulyk.Infrastructure.Data;

namespace Vulyk.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Chat> CreateUserChatAsync(string userId, string userToAddId)
        {
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

                return chat;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Chat?> GetUserChatAsync(string userId, string userToAddId)
        {
            return await _context.Chat
                .Where(c => c.UserChats
                .Any(uc => uc.UserId == userId) && c.UserChats
                .Any(uc => uc.UserId == userToAddId))
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<ChatListDto> GetChatsAsync(string userId, int lastMessageLength)
        {
            return new ChatListDto
            {
                ChatItems = await _context.UserChat
                    .Where(uc => uc.UserId == userId)
                    .Select(uc => new
                    {
                        uc.ChatId,

                        PartnerId = _context.UserChat
                            .Where(x => x.ChatId == uc.ChatId && x.UserId != userId)
                            .Select(uc => uc.UserId)
                            .FirstOrDefault()!,

                        LastMessage = _context.Message
                                .Where(m => m.ChatId == uc.ChatId)
                                .Select(m => new { m.Text, m.CreationDateTime })
                                .OrderByDescending(m => m.CreationDateTime)
                                .FirstOrDefault()
                    })
                    .Where(c => c.PartnerId != null)
                    .OrderByDescending(uc => uc.LastMessage != null ? uc.LastMessage.CreationDateTime : DateTime.MinValue)
                    .Select(uc => new ChatListItemDto
                    {
                        ChatId = uc.ChatId,
                        UserId = uc.PartnerId,
                        FullName = _context.ApplicationUser
                            .Where(u => u.Id == uc.PartnerId)
                            .Select(u => u.FullName)
                            .FirstOrDefault()!,
                        LastMessageText = uc.LastMessage != null ? uc.LastMessage.Text.Substring(0, lastMessageLength) : string.Empty,
                        LastMessageDateTime = uc.LastMessage != null ? uc.LastMessage.CreationDateTime : null
                    })
                    .ToListAsync()
            };
        }
    }
}
