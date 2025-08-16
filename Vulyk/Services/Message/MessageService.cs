using FluentResults;
using Microsoft.EntityFrameworkCore;
using Vulyk.Data;
using Vulyk.DTOs.Message;
using Vulyk.Services.Chat;
using Vulyk.Services.User;

namespace Vulyk.Services.Message
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IChatService _chatService;
        private readonly IUserService _userService;

        public MessageService(ApplicationDbContext context, IChatService chatService, IUserService userService)
        {
            _context = context;
            _chatService = chatService;
            _userService = userService;
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
            List<MessageListItemDto> messages = await _context.Message
                .Where(m => m.ChatId == dto.ChatId)
                .AsNoTracking()
                .Select(m => new MessageListItemDto
                {
                    IsMine = m.UserId == dto.UserId,
                    Text = m.Text,
                    CreationDateTime = m.CreationDateTime,

                })
                .OrderBy(m => m.CreationDateTime)
                .ToListAsync();
            return new MessageListDto
            {
                PartnerId = dto.PartnerId,
                ChatId = dto.ChatId,
                FullName = fullNameResult.Value.FullName,
                Messages = messages
            };
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

            _context.Message.Add(new Models.Message
            {
                UserId = dto.UserId,
                ChatId = createChatResult.Value.ChatId,
                Text = dto.Text,
                CreationDateTime = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync();

            return Result.Ok(new CreateMessageResultDto { ChatId = createChatResult.Value.ChatId });
        }
    }
}
