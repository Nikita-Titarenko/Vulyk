using System;
using System.Reflection.Emit;
using System.Security.Claims;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Security;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Models;
using Vulyk.ViewModels;

namespace Vulyk.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        private readonly IEmailService _emailService;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(ApplicationDbContext context, IEmailService emailService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _emailService = emailService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<AddUserResult> AddUserAsync(RegistrationDto dto, string? returnUrl)
        {
            var foundUser = await _userManager.FindByEmailAsync(dto.Email);
            if (foundUser == null)
            {
                foundUser = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
                await _userManager.CreateAsync(foundUser, dto.Password);
            }
            else if (foundUser.EmailConfirmed)
            {
                return AddUserResult.EmailAlreadyExist;
            }
            foundUser.FullName = dto.FullName;
            await _context.SaveChangesAsync();
            var token = await _userManager.GeneratePasswordResetTokenAsync(foundUser);
            await _userManager.ResetPasswordAsync(foundUser, token, dto.Password);
            await SendEmailConfirmationTokenAsync(foundUser, EmailConfirmation.ConfirmRegister, returnUrl);

            return AddUserResult.Success;
        }

        public async Task SendEmailConfirmationTokenAsync(string email, EmailConfirmation emailConfirmation)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return;
            }
            await SendEmailConfirmationTokenAsync(user, emailConfirmation, null);
        }

        public async Task SendEmailConfirmationTokenAsync(string userId, EmailConfirmation emailConfirmation, string? returnUrl)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return;
            }
            await SendEmailConfirmationTokenAsync(user, emailConfirmation, returnUrl);
        }

        private async Task SendEmailConfirmationTokenAsync(ApplicationUser user, EmailConfirmation emailConfirmation, string? returnUrl)
        {
            string? token;
            if (emailConfirmation == EmailConfirmation.ConfirmNewEmail && user.PendingNewEmail != null)
            {
                token = await _userManager.GenerateChangeEmailTokenAsync(user, user.PendingNewEmail);
            }
            else if (emailConfirmation == EmailConfirmation.ResetPassword)
            {
                token = await _userManager.GeneratePasswordResetTokenAsync(user);
            } else
            {
                token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            }

            await _emailService.SendConfirmationEmailAsync(user, token, emailConfirmation, returnUrl);
        }

        public async Task<GoogleLoginResult> ProcessExternalLoginAsync(ExternalLoginInfo info)
        {
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return GoogleLoginResult.Error;
            }
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null && !result.Succeeded)
            {
                await _userManager.AddLoginAsync(user, info);
            }

            if (user != null)
            {
                await _signInManager.SignInAsync(user, true);
                return GoogleLoginResult.Login;
            }

            user = new ApplicationUser { UserName = email, Email = email };
            var identityResult = await _userManager.CreateAsync(user);
            if (!identityResult.Succeeded)
            {
                return GoogleLoginResult.Error;
            }
            identityResult = await _userManager.AddLoginAsync(user, info);

            if (!identityResult.Succeeded)
            {
                return GoogleLoginResult.Error;
            }
            await _signInManager.SignInAsync(user, true);
            return GoogleLoginResult.Register;
        }

        public enum GoogleLoginResult
        {
            Login, Register, Error
        }

        public enum AddUserResult
        {
            Success, EmailAlreadyExist
        }

        public enum EmailConfirmation
        {
            ConfirmRegister, ConfirmCurrentEmail, ConfirmNewEmail, ResetPassword
        }

        public async Task<bool> CheckVerificationTokenAsync(EmailConfirmDto dto, EmailConfirmation emailConfirmation)
        {
            ApplicationUser? foundUser = await _userManager.FindByIdAsync(dto.UserId);
            if (foundUser == null)
            {
                return false;
            }

            IdentityResult? result = null;
            if (emailConfirmation == EmailConfirmation.ConfirmRegister || emailConfirmation == EmailConfirmation.ConfirmCurrentEmail)
            {
                result = await _userManager.ConfirmEmailAsync(foundUser, dto.VerificationToken);
            }
            else if (foundUser.PendingNewEmail != null && emailConfirmation == EmailConfirmation.ConfirmNewEmail)
            {
                result = await _userManager.ChangeEmailAsync(foundUser, foundUser.PendingNewEmail, dto.VerificationToken);
                if (result == IdentityResult.Success) {
                    result = await _userManager.SetUserNameAsync(foundUser, foundUser.PendingNewEmail);
                }
            }


            if (result == null || !result.Succeeded)
            {
                return false;
            }

            if (emailConfirmation == EmailConfirmation.ConfirmCurrentEmail)
            {
                foundUser.PendingNewEmail = dto.NewEmail;
                await _context.SaveChangesAsync();
            }
            await _signInManager.SignInAsync(foundUser, true);

            return true;
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

        public async Task<EditPasswordResult> AddPasswordAsync(string userId, string newPassword, string newPasswordConfirm)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return EditPasswordResult.UserNotFound;
            }

            if (newPassword != newPasswordConfirm)
            {
                return EditPasswordResult.newPasswordsIsDifferent;
            }

            if (await _userManager.HasPasswordAsync(user))
            {
                return EditPasswordResult.PasswordAlreadyExist;
            }
            var result = await _userManager.AddPasswordAsync(user, newPasswordConfirm);

            user.EmailConfirmed = true;
            await _context.SaveChangesAsync();

            return EditPasswordResult.Success;
        }

        public async Task<EditPasswordResult> EditPasswordByCurrentPasswordAsync(string userId, string currentPassword, string newPassword, string newPasswordConfirm)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return EditPasswordResult.UserNotFound;
            }

            if (newPassword != newPasswordConfirm)
            {
                return EditPasswordResult.newPasswordsIsDifferent;
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPasswordConfirm);
            if (!result.Succeeded)
            {
                return EditPasswordResult.CurrentPasswordOrTokenIncorrect;
            }

            return EditPasswordResult.Success;
        }

        public async Task<EditPasswordResult> ResetPasswordAsync(string userId, string token, string newPassword, string newPasswordConfirm)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return EditPasswordResult.UserNotFound;
            }

            if (newPassword != newPasswordConfirm)
            {
                return EditPasswordResult.newPasswordsIsDifferent;
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPasswordConfirm);
            if (!result.Succeeded)
            {
                return EditPasswordResult.CurrentPasswordOrTokenIncorrect;
            }
            await _signInManager.SignInAsync(user, true);
            user.EmailConfirmed = true;
            await _context.SaveChangesAsync();
            return EditPasswordResult.Success;
        }

        public enum EditPasswordResult
        {
            Success, CurrentPasswordOrTokenIncorrect, newPasswordsIsDifferent, UserNotFound, PasswordAlreadyExist
        }

        public async Task<FindUserResult> LoginAsync(string email, string password, string? returnUrl)
        {
            ApplicationUser? foundUser = await _userManager.FindByEmailAsync(email);

            if (foundUser == null)
            {
                return FindUserResult.LoginFailed;
            }
            if (!foundUser.EmailConfirmed)
            {
                await SendEmailConfirmationTokenAsync(foundUser, EmailConfirmation.ConfirmRegister, returnUrl);
                return FindUserResult.EmailEntered;
            }
            if (!await _userManager.HasPasswordAsync(foundUser))
            {
                await _userManager.AddPasswordAsync(foundUser, password);
                await SendEmailConfirmationTokenAsync(foundUser, EmailConfirmation.ConfirmRegister, returnUrl);
                return FindUserResult.EmailEntered;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(foundUser, password, false);

            if (result.IsNotAllowed)
            {
                return FindUserResult.EmailEntered;
            }

            if (!result.Succeeded)
            {
                return FindUserResult.LoginFailed;
            }

            await _signInManager.SignInAsync(foundUser, true);

            return FindUserResult.EmailConfirmed;
        }

        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
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

        public async Task<string?> GetEmailAsync(string id)
        {
            var foundUser = await _userManager.FindByIdAsync(id);
            if (foundUser == null)
            {
                return null;
            }
            return await _userManager.GetEmailAsync(foundUser);
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
