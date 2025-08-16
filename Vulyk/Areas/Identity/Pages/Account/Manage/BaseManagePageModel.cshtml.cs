using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vulyk.Services.User;

namespace Vulyk.Areas.Identity.Pages.Account.Manage
{
    public class BaseManagePageModel : BaseAccountPageModel
    {
        public BaseManagePageModel(IUserService userService, IMapper mapper, IEmailSender emailSender) : base(userService, mapper, emailSender) { }

        public string GetUserId()
        {
            //If the user is not authorized, he will be redirected to the login page
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }
    }
}