using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vulyk.Application.Services.Chat;
using Vulyk.Application.Services.User;
using Vulyk.Web.ApiModels.Responds;
using Vulyk.Web.ViewModels.Chat;

namespace Vulyk.Web.Areas.Api.Controllers
{
    [Route("api/chats")]
    [ApiController]
    public class ChatApiController : BaseApiController
    {
        private readonly IChatService _chatService;

        private readonly IMapper _mapper;

        public ChatApiController(IChatService chatService, IMapper mapper)
        {
            _chatService = chatService;
            _mapper = mapper;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> GetChats()
        {
            var getChatsResult = await _chatService.GetChatsAsync(GetUserId()!);

            if (!getChatsResult.IsSuccess)
            {
                foreach (var error in getChatsResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }

                return NotFound();
            }
            var chatListViewModel = _mapper.Map<ChatListResponseModel>(getChatsResult.Value);

            return Ok(chatListViewModel);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> Chat(string email)
        {
            if (email == null)
            {
                return BadRequest(email);
            }

            if (User.FindFirstValue(ClaimTypes.Email) == email)
            {
                ModelState.AddModelError(string.Empty, "You don't can add yourself");
                return Conflict(email);
            }

            string userId = GetUserId()!;

            var getUserChatResult = await _chatService.GetUserChatByEmailAsync(userId, email);

            if (!getUserChatResult.IsSuccess)
            {
                foreach (var error in getUserChatResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }

                return NotFound();
            }

            return Ok(_mapper.Map<GetUserChatResponseModel>(getUserChatResult.Value));
        }
    }
}
