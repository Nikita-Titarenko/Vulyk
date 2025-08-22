using AutoMapper;
using Vulyk.Web.Filters;
using Vulyk.Application.Services.User;

namespace Vulyk.Web.Areas.Identity.Pages.Account
{
    [DenyAuthenticated]
    public class AnonymousOnlyModel : BaseAccountPageModel
    {
        public AnonymousOnlyModel(IUserService userService, IMapper mapper, IEmailSender emailSender) : base(userService, mapper, emailSender) { }
    }
}
