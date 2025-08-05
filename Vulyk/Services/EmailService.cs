using System.Net.Mail;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using MimeKit;
using Org.BouncyCastle.Crypto;
using Vulyk.Controllers;

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

        public async Task SendConfirmationEmailAsync(IdentityUser user, string token, string? returnUrl)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext!);
            var callBackUrl = urlHelper.Action(nameof(AccountController.ConfirmEmail), "Account", new { userId = user.Id, token, returnUrl }, _httpContextAccessor.HttpContext!.Request.Scheme);
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Vulyk", email));
            message.To.Add(new MailboxAddress(string.Empty, user.Email));
            message.Subject = "Confirm Auth in Vulyk";
            message.Body = new TextPart("html")
            {
                Text = $"<h2>{callBackUrl}</h2>"
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
