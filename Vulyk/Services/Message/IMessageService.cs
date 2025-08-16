using FluentResults;
using Vulyk.DTOs.Message;

namespace Vulyk.Services
{
    public interface IMessageService
    {
        Task<Result<CreateMessageResultDto>> CreateMessageAsync(CreateMessageDto dto);
        Task<Result<MessageListDto>> GetMessagesAsync(GetMessagesDto dto);
    }
}