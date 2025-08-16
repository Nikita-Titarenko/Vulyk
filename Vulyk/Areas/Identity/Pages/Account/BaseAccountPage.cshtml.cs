using AutoMapper;
using Vulyk.DTOs.Account;
using Vulyk.Services.User;

namespace Vulyk.Areas.Identity.Pages.Account
{
    public class BaseAccountPageModel : PageModel
    {
        public readonly IUserService _userService;

        public readonly IMapper _mapper;

        public readonly IEmailSender _emailSender;

        public BaseAccountPageModel(IUserService userService, IMapper mapper, IEmailSender emailSender)
        {
            _userService = userService;
            _mapper = mapper;
            _emailSender = emailSender;
        }

        public string CreateEmailConfirmationLink(ConfirmTokenDto authResultDto, string redirectPage, string? returnUrl = null)
        {
            return Url.Page(
redirectPage,
pageHandler: null,
values: new { area = "Identity", userId = authResultDto.UserId, code = authResultDto.Code, returnUrl = returnUrl },
protocol: Request.Scheme)!;
        }
    }
}