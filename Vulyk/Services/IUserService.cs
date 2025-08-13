using FluentResults;
using Microsoft.AspNetCore.Identity;
using Vulyk.DTOs;
using Vulyk.Entities;
using static Vulyk.Services.UserService;

namespace Vulyk.Services
{
    public interface IUserService
    {
        Task<Result> EditUserProfileAsync(UserProfileEditDto dto);
        Task<Result<FindUserByEmailDto>> FindUserByEmailAsync(string email);
        Task<Result<UserProfileEditDto>> GetUserProfileAsync(string id);
        Task<Result<GetFullNameResultDto>> GetFullNameAsync(string id);
        Task<Result> EditFullNameAsync(string id, string fullName);
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
    }
}