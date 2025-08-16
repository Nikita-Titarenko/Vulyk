using FluentResults;
using Microsoft.AspNetCore.Identity;
using Vulyk.DTOs.Account;
using Vulyk.DTOs.Chat;
using Vulyk.DTOs.Profile;
using Vulyk.DTOs.UserManagement;
using Vulyk.Models;
using static Vulyk.Services.User.UserService;

namespace Vulyk.Services.User
{
    public interface IUserService
    {
        Task<Result> EditUserProfileAsync(UserProfileEditDto dto);
        Task<Result<FindUserByEmailDto>> FindUserByEmailAsync(string email);
        Task<Result<UserProfileEditDto>> GetUserProfileAsync(string userId);
        Task<Result<GetFullNameResultDto>> GetFullNameAsync(string userId);
        Task<Result> EditFullNameAsync(string userId, string fullName);
        Task<Result<ExternalLoginResultDto>> ProcessExternalLoginAsync(ExternalLoginInfo info);
        Task<Result> SetPasswordAsync(SetPasswordDto dto);
        Task<Result> ChangePasswordAsync(ChangePasswordDto dto);
        Task<Result> ResetPasswordAsync(ResetPasswordDto dto);
        Task<Result<AuthResultDto>> RegisterAsync(RegisterDto registrationDto);
        Task<Result<AuthResultDto>> LoginAsync(LoginDto dto);
        Task<Result> ConfirmEmailAsync(ConfirmTokenDto dto);
        Task<Result<ConfirmTokenDto>> GeneratePasswordResetTokenAsync(string email);
        Task<Result<bool>> HasPasswordAsync(string userId);
        Task<Result<ConfirmTokenDto>> GenerateCurrentEmailConfirmationTokenByIdAsync(string userId);
        Task<Result<ConfirmTokenDto>> GenerateCurrentEmailConfirmationTokenByEmailAsync(string email);
        Task<Result> ConfirmCurrentEmailAsync(ConfirmTokenDto dto);
        Task<Result<ConfirmTokenDto>> GenerateNewEmailConfirmationTokenAsync(string userId);
        Task<Result> ConfirmNewEmailAsync(ConfirmTokenDto dto);
        Task<Result<UsersDto>> GetUsers(int page, int pageSize);
    }
}