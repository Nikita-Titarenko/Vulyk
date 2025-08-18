using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Vulyk.ApiModels.Requests;
using Vulyk.ApiModels.Responds;
using Vulyk.DTOs.Account;
using Vulyk.Services.JwtToken;
using Vulyk.Services.User;

namespace Vulyk.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        public readonly IUserService _userService;

        public readonly IMapper _mapper;

        public readonly IEmailSender _emailSender;

        private readonly IJwtTokenService _jwtTokenService;

        public UserApiController(IUserService userService, IMapper mapper, IEmailSender emailSender, IJwtTokenService jwtTokenService)
        {
            _userService = userService;
            _mapper = mapper;
            _emailSender = emailSender;
            _jwtTokenService = jwtTokenService;
        }

        public string CreateEmailConfirmationLink(ConfirmTokenDto authResultDto, string redirectPage, string? returnUrl = null)
        {
            return Url.Page(
redirectPage,
pageHandler: null,
values: new { area = "Identity", userId = authResultDto.UserId, code = authResultDto.Code, returnUrl = returnUrl },
protocol: Request.Scheme)!;
        }

        public async Task<IActionResult> Register(RegisterApiModel apiModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _userService.RegisterAsync(_mapper.Map<RegisterDto>(apiModel));

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Email is already taken");
                return Conflict();
            }

            var callbackUrl = CreateEmailConfirmationLink(_mapper.Map<ConfirmTokenDto>(result.Value), "/Account/ConfirmEmail", null);

            await _emailSender.SendEmailAsync(apiModel.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return CreatedAtAction(string.Empty, new RegisterResponseDto{UserId = result.Value.UserId, Message = "Confirm your email to verificate your account"});
        }
    }
}
