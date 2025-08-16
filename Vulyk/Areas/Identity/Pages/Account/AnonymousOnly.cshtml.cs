using AutoMapper;
using Vulyk.Filters;
using Vulyk.Services.User;

namespace Vulyk.Areas.Identity.Pages.Account
{
    [DenyAuthenticated]
    public class AnonymousOnlyModel : BaseAccountPageModel
    {
        public AnonymousOnlyModel(IUserService userService, IMapper mapper, IEmailSender emailSender) : base(userService, mapper, emailSender) { }
    }
}
