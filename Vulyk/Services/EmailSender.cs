using MimeKit;

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
