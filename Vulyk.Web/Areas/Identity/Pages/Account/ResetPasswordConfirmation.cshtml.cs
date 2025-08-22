using AutoMapper;
using Vulyk.Application.Services.User;

namespace Vulyk.Web.Areas.Identity.Pages.Account
{
    public class ResetPasswordConfirmationModel : AnonymousOnlyModel
    {
        public ResetPasswordConfirmationModel(IUserService userService, IMapper mapper, IEmailSender emailSender) : base(userService, mapper, emailSender) { }

        public void OnGet()
        {
        }
    }
}
