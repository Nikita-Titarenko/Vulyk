using AutoMapper;
using FluentResults;
using Vulyk.Application.DTOs.Message;
using Vulyk.Application.Repositories;
using Vulyk.Application.Services.Chat;
using Vulyk.Application.Services.User;

namespace Vulyk.Application.Services.Message
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public MessageService(IMessageRepository messageRepository, IChatService chatService, IUserService userService, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _chatService = chatService;
            _userService = userService;
            _mapper = mapper;
        }

        ///  <summary>
        ///  Get messages from Chat
        ///  </summary>
        ///  <param name="GetMessagesDto">The data containing UserId, ChatId and PartnerId</param>
        ///  <returns>
        ///  <see cref="MessageListDto"/> containing:
        ///  <list type="bullet">
        ///  <item>PartnerId, ChatId, Messages and parner's FullName if operation successful</item>
        ///  <item>Error information if users not found</item>
        /// </list>
        /// </returns>
        public async Task<Result<MessageListDto>> GetMessagesAsync(GetMessagesDto dto)
        {
            var fullNameResult = await _userService.GetFullNameAsync(dto.PartnerId);
            if (!fullNameResult.IsSuccess)
            {
                return Result.Fail(fullNameResult.Errors);
            }
            var messages = (await _messageRepository.GetMessagesAsync(dto.UserId, dto.PartnerId, dto.ChatId)).ToList();
            var messageListDto = _mapper.Map<IEnumerable<Domain.Models.Message>, MessageListDto>(messages);
            messageListDto.PartnerId = dto.PartnerId;
            for (int i = 0; i < messages.Count(); i++)
            {
                messageListDto.Messages[i].IsMine = messages[i].UserId == dto.UserId;
            }
            messageListDto.FullName = fullNameResult.Value.FullName;
            return messageListDto;
        }

        ///  <summary>
        ///  Create message
        ///  </summary>
        ///  <param name="CreateMessageDto">The data containing UserId, MessageText and PartnerId</param>
        ///  <returns>
        ///  <see cref="CreateMessageResultDto"/> containing:
        ///  <list type="bullet">
        ///  <item>ChatId if operation successful</item>
        ///  <item>Error information if users not found</item>
        /// </list>
        /// </returns>
        public async Task<Result<CreateMessageResultDto>> CreateMessageAsync(CreateMessageDto dto)
        {
            var createChatResult = await _chatService.CreateUserChatAsync(dto.UserId, dto.PartnerId);

            if (!createChatResult.IsSuccess)
            {
                return Result.Fail(createChatResult.Errors);
            }
            await _messageRepository.CreateMessageAsync(dto.UserId, createChatResult.Value.ChatId, dto.Text);

            return Result.Ok(new CreateMessageResultDto { ChatId = createChatResult.Value.ChatId });
        }
    }
}
