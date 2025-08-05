using Microsoft.AspNetCore.Identity;
using Vulyk.DTOs;
using static Vulyk.Services.UserService;

namespace Vulyk.Services
{
    public interface IUserService
    {
        Task<UserService.AddUserResult> AddUserAsync(RegistrationDto dto);
        Task<bool> CheckVerificationTokenAsync(EmailConfirmDto dto);
        Task EditUserAsync(string userId, UserEditDto dto);
        Task<(string?, UserService.FindUserResult)> FindUserByEmailAsync(string email);
        Task<UserEditDto> FindUserByIdAsync(string id);
        Task<string?> GetFullNameAsync(string id);
        Task<FindUserResult> LoginAsync(string email, string password);
        Task<string?> GetEmailAsync(string id);
        Task LogOutAsync();
        Task EditFullNameAsync(string id, string fullName);
        Task<GoogleLoginResult> ProcessExternalLoginAsync(ExternalLoginInfo info);
    }
}