
using Microsoft.AspNetCore.Identity;
using Vulyk.Models;
using static Vulyk.Services.UserService;

namespace Vulyk.Services
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(ApplicationUser user, string token, EmailConfirmation emailConfirmation, string? returnUrl);
    }
}