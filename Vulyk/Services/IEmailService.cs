
using Microsoft.AspNetCore.Identity;

namespace Vulyk.Services
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(IdentityUser user, string token);
    }
}