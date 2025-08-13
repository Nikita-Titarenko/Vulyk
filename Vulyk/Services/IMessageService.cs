using FluentResults;
using Vulyk.DTOs;

namespace Vulyk.Services
{
    public interface IMessageService
    {
        Task<Result<CreateMessageResultDto>> CreateMessageAsync(CreateMessageDto dto);
        Task<Result<MessageListDto>> GetMessagesAsync(GetMessagesDto dto);
    }
}