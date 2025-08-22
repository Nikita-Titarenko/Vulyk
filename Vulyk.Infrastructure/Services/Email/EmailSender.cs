using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using Vulyk.Infrastructure.Settings;

namespace Vulyk.Infrastructure.Services.Email
{
    public class EmailSender : IEmailSender
    {

        private readonly IOptions<EmailSettings> _options;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _options = emailSettings;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string fromAddress = _options.Value.Email;
            string password = _options.Value.Password;
            string host = _options.Value.Server;
            int port = _options.Value.Port;
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
