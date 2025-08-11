using System;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Text;
using FluentResults;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Security;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Entities;
using Vulyk.ViewModels;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Vulyk.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            ILogger<UserService> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<Result<AuthResultDto>> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, FullName = dto.FullName };

            var result = await _userManager.CreateAsync(user, dto.Password);

            string userId;

            if (!result.Succeeded)
            {
                if (!result.Errors.Any(e => e.Code == "DuplicateUserName"))
                {
                    return Result.Fail(result.Errors.Select(e => new Error(e.Description).WithMetadata("Code", e.Code)));
                }

                user = await _userManager.FindByEmailAsync(dto.Email);

                if (user!.EmailConfirmed)
                {
                    return Result.Fail(result.Errors.Select(e => new Error(e.Description).WithMetadata("Code", e.Code)));
                }

                if (!await _userManager.CheckPasswordAsync(user, dto.Password))
                {
                    return Result.Fail(result.Errors.Select(e => new Error(e.Description).WithMetadata("Code", e.Code)));
                }

                userId = user.Id;
                _logger.LogInformation("User trying login again.");
            }
            else
            {
                _logger.LogInformation("User created a new account with password.");
                userId = await _userManager.GetUserIdAsync(user);
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            return Result.Ok(new AuthResultDto { UserId = userId, Code = code });
        }

        public async Task<Result<AuthResultDto>> LoginAsync(LoginDto dto)
        {
            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, true, false);

            if (result.Succeeded)
            {
                return Result.Ok(new AuthResultDto { EmailNotConfirmed = false });
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return Result.Fail(new Error("Login failed").WithMetadata("Code", "LoginFailed"));
            }

            bool isPasswordCorrect = false;
            if (result.IsNotAllowed)
            {
                isPasswordCorrect = await _userManager.CheckPasswordAsync(user, dto.Password);
            }

            string code;

            if (isPasswordCorrect)
            {
                code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                return Result.Ok(new AuthResultDto { UserId = user.Id, Code = code, EmailNotConfirmed = true });
            }

            var hasPasswordResult = await _userManager.HasPasswordAsync(user);

            if (hasPasswordResult)
            {
                return Result.Fail(new Error("Login failed").WithMetadata("Code", "LoginFailed"));
            }

            code = (await GeneratePasswordResetTokenAsync(dto.Email)).Value.Code;
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            return Result.Ok(new AuthResultDto { UserId = user.Id, Code = code, PasswordNotExist = true });
        }

        public async Task<Result> ConfirmEmailAsync(ConfirmTokenDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }
            dto.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Code));
            var result = await _userManager.ConfirmEmailAsync(user, dto.Code);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.Select(e => new Error(e.Description).WithMetadata("Code", e.Code)));
            }
            await _signInManager.SignInAsync(user, new AuthenticationProperties());
            return Result.Ok();
        }

        public async Task<Result<ConfirmTokenDto>> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !user.EmailConfirmed)
            {
                return Result.Fail(new Error("Invalid request").WithMetadata("Code", "InvalidRequest"));
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            return Result.Ok(new ConfirmTokenDto {Code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code)), UserId = user.Id});
        }

        public async Task<Result<ConfirmTokenDto>> GenerateCurrentEmailConfirmationTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.EmailConfirmed)
            {
                return Result.Fail(new Error("Invalid request").WithMetadata("Code", "InvalidRequest"));
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return Result.Ok(new ConfirmTokenDto { Code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code)), UserId = user.Id });
        }

        public async Task<Result<ConfirmTokenDto>> ConfirmCurrentEmailAsync(ConfirmTokenDto dto, string? newEmail)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }
            dto.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Code));
            var result = await _userManager.ConfirmEmailAsync(user, dto.Code);
            if (!result.Succeeded)
            {
                return Result.Fail(new Error("Token incorrect").WithMetadata("Code", "TokenIncorrect"));
            }
            if (newEmail != null)
            {
                user.PendingNewEmail = newEmail;
                await _userManager.UpdateAsync(user);
            }

            return Result.Ok();
        }

        public async Task<Result<ConfirmTokenDto>> GenerateNewEmailConfirmationTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.EmailConfirmed)
            {
                return Result.Fail(new Error("Invalid request").WithMetadata("Code", "InvalidRequest"));
            }

            if (user.PendingNewEmail == null)
            {
                return Result.Fail(new Error("Current email not confirmed").WithMetadata("Code", "CurrentEmailNotConfirmed"));
            }

            var code = await _userManager.GenerateChangeEmailTokenAsync(user, user.PendingNewEmail);
            return Result.Ok(new ConfirmTokenDto { Code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code)), UserId = user.Id });
        }

        public async Task<Result<ConfirmTokenDto>> ConfirmNewEmailAsync(ConfirmTokenDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            if (user.PendingNewEmail == null)
            {
                return Result.Fail(new Error("Current email not confirmed").WithMetadata("Code", "CurrentEmailNotConfirmed"));
            }

            dto.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Code));
            var result = await _userManager.ChangeEmailAsync(user, user.PendingNewEmail, dto.Code);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.Select(e => new Error(e.Description).WithMetadata("Code", e.Code)));
            }
            await _signInManager.RefreshSignInAsync(user);

            return Result.Ok();
        }

        public async Task<Result<bool>> HasPasswordAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            bool result = await _userManager.HasPasswordAsync(user);

            return Result.Ok(result);
        }


        public async Task<Result<ExternalLoginResultDto>> ProcessExternalLoginAsync(ExternalLoginInfo info)
        {
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
            if (result.Succeeded)
            {
                return Result.Ok(new ExternalLoginResultDto { IsLogin = true });
            }
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return Result.Fail(new Error("Invalid request").WithMetadata("Code", "InvalidRequest"));
            }
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, true);
                user.EmailConfirmed = true;
                await _context.SaveChangesAsync();
                return Result.Ok(new ExternalLoginResultDto { IsLogin = true });
            }
            var fullName = info.Principal.FindFirstValue(ClaimTypes.Name);
            user = new ApplicationUser { UserName = email, Email = email, FullName = fullName, EmailConfirmed = true };
            var identityResult = await _userManager.CreateAsync(user);
            if (!identityResult.Succeeded)
            {
                return Result.Fail(new Error("Invalid request").WithMetadata("Code", "InvalidRequest"));
            }
            identityResult = await _userManager.AddLoginAsync(user, info);

            if (!identityResult.Succeeded)
            {
                return Result.Fail(new Error("Invalid request").WithMetadata("Code", "InvalidRequest"));
            }
            await _signInManager.SignInAsync(user, true);
            return Result.Ok(new ExternalLoginResultDto { IsLogin = false });
        }

        public async Task EditUserProfileAsync(string userId, UserProfileEditDto dto)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return;
            }

            user.PhoneNumber = dto.Phone?.Trim();
            user.FullName = dto.FullName.Trim();
            await _context.SaveChangesAsync();
        }

        public async Task<Result> SetPasswordAsync(string userId, string newPassword, string newPasswordConfirm)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            if (newPassword != newPasswordConfirm)
            {
                return Result.Fail(new Error("Password is differents").WithMetadata("Code", "PasswordIsDifferents"));
            }

            if (await _userManager.HasPasswordAsync(user))
            {
                return Result.Fail(new Error("Password already exist").WithMetadata("Code", "PasswordAlreadyExist"));
            }
            var result = await _userManager.AddPasswordAsync(user, newPasswordConfirm);

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User set their password successfully.");
            return Result.Ok();
        }

        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return Result.Fail(new Error("Password is differents").WithMetadata("Code", "PasswordIsDifferents"));
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.ConfirmPassword);
            if (!result.Succeeded)
            {
                return Result.Fail(new Error("Current password incorrect").WithMetadata("Code", "CurrentPasswordIncorrect"));
            }

            await _signInManager.RefreshSignInAsync(user);

            _logger.LogInformation("User changed their password successfully.");

            return Result.Ok();
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordDto dto)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            if (dto.Password != dto.ConfirmPassword)
            {
                return Result.Fail(new Error("Password is differents").WithMetadata("Code", "PasswordIsDifferents"));
            }

            var result = await _userManager.ResetPasswordAsync(user, dto.Code, dto.ConfirmPassword);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.Select(e => new Error(e.Description).WithMetadata("Code", "PasswordIsDifferents")));
            }

            return Result.Ok();
        }

        public async Task<UserProfileEditDto> FindUserByIdAsync(string id)
        {
            var foundUser = await _userManager.FindByIdAsync(id);
            if (foundUser == null)
            {
                throw new InvalidParameterException();
            }
            var isPasswordExist = await _userManager.HasPasswordAsync(foundUser);
            UserProfileEditDto? user = await _context.ApplicationUser
                .Where(u => u.Id == id && (u.EmailConfirmed || !isPasswordExist))
                .Select(u => new UserProfileEditDto { Email = u.Email!, FullName = u.FullName!, Phone = u.PhoneNumber, IsPasswordExist = isPasswordExist })
                .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new InvalidKeyException();
            }

            return user;
        }

        public async Task<string?> GetFullNameAsync(string id)
        {
            return await _context.ApplicationUser.Where(u => u.Id == id && u.EmailConfirmed).Select(u => u.FullName).FirstOrDefaultAsync();
        }

        public async Task EditFullNameAsync(string id, string fullName)
        {
            var foundUser = await _userManager.FindByIdAsync(id);
            if (foundUser == null)
            {
                return;
            }

            foundUser.FullName = fullName;
            await _context.SaveChangesAsync();
        }

        public async Task<(string?, FindUserResult)> FindUserByEmailAsync(string email)
        {
            string emailNormalized = email.ToLower();
            ApplicationUser? foundUser = await _userManager.FindByEmailAsync(email);

            if (foundUser == null)
            {
                return (null, FindUserResult.LoginFailed);
            }

            if (foundUser.EmailConfirmed)
            {
                return (foundUser.Id, FindUserResult.EmailConfirmed);
            }
            return (foundUser.Id, FindUserResult.EmailEntered);
        }

        public enum FindUserResult
        {
            EmailEntered, EmailConfirmed, LoginFailed
        }
    }
}
