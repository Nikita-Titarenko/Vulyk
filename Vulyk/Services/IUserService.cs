using FluentResults;
using Microsoft.AspNetCore.Identity;
using Vulyk.DTOs;
using Vulyk.Entities;
using static Vulyk.Services.UserService;

namespace Vulyk.Services
{
    public interface IUserService
    {
        Task EditUserProfileAsync(string userId, UserProfileEditDto dto);
        Task<(string?, UserService.FindUserResult)> FindUserByEmailAsync(string email);
        Task<UserProfileEditDto> FindUserByIdAsync(string id);
        Task<string?> GetFullNameAsync(string id);
        Task EditFullNameAsync(string id, string fullName);
        Task<Result<ExternalLoginResultDto>> ProcessExternalLoginAsync(ExternalLoginInfo info);
        Task<Result> SetPasswordAsync(string userId, string newPassword, string newPasswordConfirm);
        Task<Result> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<Result> ResetPasswordAsync(ResetPasswordDto dto);
        Task<Result<AuthResultDto>> RegisterAsync(RegisterDto registrationDto);
        Task<Result<AuthResultDto>> LoginAsync(LoginDto dto);
        Task<Result> ConfirmEmailAsync(ConfirmTokenDto dto);
        Task<Result<ConfirmTokenDto>> GeneratePasswordResetTokenAsync(string email);
        Task<Result<bool>> HasPasswordAsync(string userId);
        Task<Result<ConfirmTokenDto>> GenerateCurrentEmailConfirmationTokenAsync(string userId);
        Task<Result<ConfirmTokenDto>> ConfirmCurrentEmailAsync(ConfirmTokenDto dto, string? newEmail);
        Task<Result<ConfirmTokenDto>> GenerateNewEmailConfirmationTokenAsync(string userId);
        Task<Result<ConfirmTokenDto>> ConfirmNewEmailAsync(ConfirmTokenDto dto);
    }
}