using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vulyk.Services.User;

namespace Vulyk.Areas.Identity.Pages.Account
{
    public class ResetPasswordConfirmationModel : AnonymousOnlyModel
    {
        public ResetPasswordConfirmationModel(IUserService userService, IMapper mapper, IEmailSender emailSender) : base(userService, mapper, emailSender) { }

        public void OnGet()
        {
        }
    }
}
