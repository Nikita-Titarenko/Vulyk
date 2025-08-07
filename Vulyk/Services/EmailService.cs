using System.Net.Mail;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using MimeKit;
using Org.BouncyCastle.Crypto;
using Vulyk.Controllers;
using Vulyk.Models;
using static Vulyk.Services.UserService;

namespace Vulyk.Services
{
    public class EmailService : IEmailService
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string email = "vulyk.messenger@gmail.com";
        private const string password = "bdfomjzqtrkhqojn";

        public EmailService(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor, IHttpContextAccessor httpContextAccessor)
        {
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SendConfirmationEmailAsync(ApplicationUser user, string token, EmailConfirmation emailConfirmation, string? returnUrl)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext!);
            string? action = null;
            string? controller = null;
            var message = new MimeMessage();
            if (emailConfirmation == EmailConfirmation.ConfirmCurrentEmail)
            {
                action = nameof(ProfileController.NewEmailInput);
                controller = "Profile";
                message.Subject = "Confirm changing email in Vulyk";
            }
            else if (emailConfirmation == EmailConfirmation.ResetPassword)
            {
                action = nameof(AccountController.ResetPassword);
                controller = "Account";
                message.Subject = "Confirm reset password in Vulyk";
            }
            else if (emailConfirmation == EmailConfirmation.ConfirmRegister || emailConfirmation == EmailConfirmation.ConfirmLogin)
            {
                action = nameof(AccountController.ConfirmEmail);
                controller = "Account";
                message.Subject = "Confirm Registration in Vulyk";
            }
            else if (emailConfirmation == EmailConfirmation.ConfirmNewEmail)
            {
                action = nameof(AccountController.ConfirmEmail);
                controller = "Account";
                message.Subject = "Confirm changing email in Vulyk";
            }

            var callBackUrl = urlHelper.Action(action, controller, new { userId = user.Id, token, emailConfirmation, returnUrl }, _httpContextAccessor.HttpContext!.Request.Scheme);

            message.From.Add(new MailboxAddress("Vulyk", email));
            message.To.Add(new MailboxAddress(string.Empty, emailConfirmation == EmailConfirmation.ConfirmNewEmail ? user.PendingNewEmail : user.Email));

            message.Body = new TextPart("html")
            {
                Text = $"<h1>{"Click below to verify your account."}</h1><h2>{callBackUrl}</h2>"
            };
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(email, password);
                smtp.Send(message);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
