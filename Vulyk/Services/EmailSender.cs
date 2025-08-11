using System.Net.Mail;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using MimeKit;
using Org.BouncyCastle.Crypto;
using Vulyk.Controllers;
using Vulyk.Entities;
using static Vulyk.Services.UserService;

namespace Vulyk.Services
{
    public class EmailSender : IEmailSender
    {

        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //if (emailConfirmation == EmailConfirmation.ConfirmCurrentEmail)
            //{
            //    action = nameof(ProfileController.NewEmailInput);
            //    controller = "Profile";
            //    message.Subject = "Confirm changing email in Vulyk";
            //}
            //else if (emailConfirmation == EmailConfirmation.ResetPassword)
            //{
            //    action = nameof(AccountController.ResetPassword);
            //    controller = "Account";
            //    message.Subject = "Confirm reset password in Vulyk";
            //}
            //else if (emailConfirmation == EmailConfirmation.ConfirmRegister || emailConfirmation == EmailConfirmation.ConfirmLogin)
            //{
            //    action = nameof(AccountController.ConfirmEmail);
            //    controller = "Account";
            //    message.Subject = "Confirm Registration in Vulyk";
            //}
            //else if (emailConfirmation == EmailConfirmation.ConfirmNewEmail)
            //{
            //    action = nameof(AccountController.ConfirmEmail);
            //    controller = "Account";
            //    message.Subject = "Confirm changing email in Vulyk";
            //}
            string? fromAddress = _configuration["EmailSettings:Email"];
            string? password = _configuration["EmailSettings:Password"];
            string? host = _configuration["EmailSettings:Server"];
            int port = Convert.ToInt32(_configuration["EmailSettings:Port"]);
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Vulyk", fromAddress));
            message.To.Add(new MailboxAddress(string.Empty, email));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = htmlMessage
            };
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                await smtp.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(fromAddress, password);
                smtp.Send(message);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
