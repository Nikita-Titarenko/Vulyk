using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using Vulyk.ApiModels.Requests;
using Vulyk.ApiModels.Responds;
using Vulyk.Controllers;
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

        private string CreateEmailConfirmationLink(ConfirmTokenDto authResultDto, string redirectPage)
        {
            return Url.Action(
                redirectPage,
                "UserApi",
                new { userId = authResultDto.UserId, code = authResultDto.Code },
                Request.Scheme)!;
        }

        private string GetUserId()
        {
            //If the user is not authorized, he will get 401 Unauthorized
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequestModel apiModel)
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

            var callbackUrl = CreateEmailConfirmationLink(_mapper.Map<ConfirmTokenDto>(result.Value), "ConfirmEmail");

            await _emailSender.SendEmailAsync(apiModel.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return CreatedAtAction(string.Empty, new AuthResponseModel{UserId = result.Value.UserId!, Message = "Confirm your email to verificate your account"});
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest();
            }

            var result = await _userService.ConfirmEmailAsync(new ConfirmTokenDto { UserId = userId, Code = code });
            if (!result.IsSuccess)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }

                return BadRequest();
            }

            return Ok(new AuthResponseModel { UserId = userId, Message = "Your email was successful confirmed. Sign in to start messaging" });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var loginDto = _mapper.Map<LoginDto>(requestModel);
            loginDto.UserIdNeed = true;
            var result = await _userService.LoginAsync(loginDto);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return BadRequest();
            }

            if (result.Value.EmailNotConfirmed)
            {
                var callbackUrl = CreateEmailConfirmationLink(_mapper.Map<ConfirmTokenDto>(result.Value), "Api/UserApi/ConfirmEmail");

                await _emailSender.SendEmailAsync(requestModel.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return CreatedAtAction(string.Empty, new AuthResponseModel { UserId = result.Value.UserId!, Message = "Confirm your email to verificate your account" });
            }

            return Ok(new LoginResponseModel { UserId = result.Value.UserId!, JwtToken = _jwtTokenService.GenerateJwtToken(result.Value.UserId!) });
        }

        [HttpGet("GetProfile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetProfile()
        {
            var getUserProfileResult = await _userService.GetUserProfileAsync(GetUserId());
            if (!getUserProfileResult.IsSuccess)
            {
                foreach (var error in getUserProfileResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }

                return NotFound();
            }

            return Ok(_mapper.Map<ProfileResponseModel>(getUserProfileResult.Value));
        }
    }
}
