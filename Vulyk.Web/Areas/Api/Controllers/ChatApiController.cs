using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vulyk.Application.Services.User;
using Vulyk.Web.ViewModels.Chat;
using Vulyk.Application.Services.Chat;

namespace Vulyk.Web.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatApiController : BaseApiController
    {
        private readonly IChatService _chatService;

        private readonly IUserService _userService;

        private readonly IMapper _mapper;

        public ChatApiController(IChatService chatService, IMapper mapper, IUserService userService)
        {
            _chatService = chatService;
            _mapper = mapper;
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserChat(EmailInputViewModel emailInputChatViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(emailInputChatViewModel);
            }

            if (User.FindFirstValue(ClaimTypes.Email) == emailInputChatViewModel.Email)
            {
                ModelState.AddModelError(string.Empty, "You don't can add yourself");
                return BadRequest(emailInputChatViewModel);
            }

            string userId = GetUserId()!;

            var getUserChatResult = await _chatService.GetUserChatByEmailAsync(userId, emailInputChatViewModel.Email);

            if (!getUserChatResult.IsSuccess)
            {
                foreach (var error in getUserChatResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }

                return NotFound();
            }

            if (getUserChatResult.Value.ChatId.HasValue)
            {
                return Ok( new { chatId = getUserChatResult.Value.ChatId, userToAddId = getUserChatResult.Value.UserId });
            }

            return Ok(new { userToAddId = getUserChatResult.Value.UserId });
        }
    }
}
