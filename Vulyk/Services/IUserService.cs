using Microsoft.AspNetCore.Identity;
using Vulyk.DTOs;
using Vulyk.Models;
using static Vulyk.Services.UserService;

namespace Vulyk.Services
{
    public interface IUserService
    {
        Task<UserService.AddUserResult> AddUserAsync(RegistrationDto dto, string? returnUrl);
        Task<bool> CheckVerificationTokenAsync(EmailConfirmDto dto, EmailConfirmation emailConfirmation);
        Task EditUserProfileAsync(string userId, UserProfileEditDto dto);
        Task<(string?, UserService.FindUserResult)> FindUserByEmailAsync(string email);
        Task<UserProfileEditDto> FindUserByIdAsync(string id);
        Task<string?> GetFullNameAsync(string id);
        Task<FindUserResult> LoginAsync(string email, string password, string? returnUrl);
        Task<string?> GetEmailAsync(string id);
        Task LogOutAsync();
        Task EditFullNameAsync(string id, string fullName);
        Task<GoogleLoginResult> ProcessExternalLoginAsync(ExternalLoginInfo info);
        Task<EditPasswordResult> AddPasswordAsync(string userId, string newPassword, string newPasswordConfirm);
        Task<EditPasswordResult> EditPasswordByCurrentPasswordAsync(string userId, string currentPassword, string newPassword, string newPasswordConfirm);
        Task<EditPasswordResult> ResetPasswordAsync(string userId, string token, string newPassword, string newPasswordConfirm);
        Task SendEmailConfirmationTokenAsync(string email, EmailConfirmation emailConfirmation);
        Task SendEmailConfirmationTokenAsync(string userId, EmailConfirmation emailConfirmation, string? returnUrl);
    }
}