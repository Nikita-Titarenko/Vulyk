using AutoMapper;
using Vulyk.Application.DTOs.Account;
using Vulyk.Application.Services.User;

namespace Vulyk.Web.Areas.Identity.Pages.Account
{
    public class BaseAccountPageModel : PageModel
    {
        protected readonly IUserService _userService;

        protected readonly IMapper _mapper;

        protected readonly IEmailSender _emailSender;

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