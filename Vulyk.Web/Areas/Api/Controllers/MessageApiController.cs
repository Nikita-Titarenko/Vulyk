using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Vulyk.Application.DTOs.Message;
using Vulyk.Application.Services.Message;
using Vulyk.Web.ApiModels.Requests;
using Vulyk.Web.ApiModels.Responds;

namespace Vulyk.Web.Areas.Api.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class MessageApiController : BaseApiController
    {
        private readonly IMessageService _messageService;

        private readonly IMapper _mapper;

        public MessageApiController(IMessageService messageService, IMapper mapper)
        {
            _messageService = messageService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetMessages(int chatId, string partnerId)
        {
            string userId = GetUserId()!;
            var dto = new GetMessagesDto { ChatId = chatId, UserId = userId, PartnerId = partnerId };
            var getMessagesResult = await _messageService.GetMessagesAsync(dto);
            if (!getMessagesResult.IsSuccess)
            {
                return NotFound(new {
                    Error = "MessageNotFound",
                    Message = "Messages for ID were not found"
                });
            }

            var messageListViewModel = _mapper.Map<MessageListResponseModel>(getMessagesResult.Value);
            messageListViewModel.ChatId = chatId;

            return Ok(messageListViewModel);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateMessage(CreateMessageRequestModel createMessageViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId = GetUserId()!;

            var dto = _mapper.Map<CreateMessageDto>(createMessageViewModel);
            dto.UserId = userId;
            var createMessageResult = await _messageService.CreateMessageAsync(dto);
            if (!createMessageResult.IsSuccess)
            {
                foreach (var error in createMessageResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }

                return BadRequest(new
                {
                    Error = "CreateMessageError",
                    Message = "The message was not created"
                });
            }

            return CreatedAtAction(nameof(GetMessages), new { createMessageResult.Value.ChatId });
        }
    }
}
