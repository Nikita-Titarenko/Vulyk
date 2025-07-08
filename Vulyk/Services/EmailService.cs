using MailKit.Net.Smtp;
using MimeKit;
using Org.BouncyCastle.Crypto;

namespace Vulyk.Services
{
    public class EmailService
    {
        private const string email = "vulyk.messenger@gmail.com";
        private const string password = "bdfomjzqtrkhqojn"; 
        public async Task SendEmailAsync(string emailToSend, string title, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Vulyk", email));
            message.To.Add(new MailboxAddress(string.Empty, emailToSend));
            message.Subject = title;
            message.Body = new TextPart("html")
            {
                Text = body
                
            };
            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(email, password);
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
            }
        }
    }
}
