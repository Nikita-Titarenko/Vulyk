using FluentResults;
using Vulyk.Application.DTOs.Message;

namespace Vulyk.Application.Services.Message
{
    public interface IMessageService
    {
        Task<Result<CreateMessageResultDto>> CreateMessageAsync(CreateMessageDto dto);
        Task<Result<MessageListDto>> GetMessagesAsync(GetMessagesDto dto);
    }
}