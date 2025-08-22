using Vulyk.Domain.Models;

namespace Vulyk.Application.Repositories
{
    public interface IMessageRepository
    {
        Task CreateMessageAsync(string userId, int chatId, string text);
        Task<IEnumerable<Message>> GetMessagesAsync(string userId, string partnerId, int chatId);
    }
}