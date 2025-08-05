using System;
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

        public async Task<AddUserResult> AddUserAsync(RegistrationDto dto)
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
            token = await _userManager.GenerateEmailConfirmationTokenAsync(foundUser);
            await _emailService.SendConfirmationEmailAsync(foundUser, token);

            return AddUserResult.Success;
        }

        public async Task<GoogleLoginResult> ProcessExternalLoginAsync(ExternalLoginInfo info)
        {
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
            if (result.Succeeded)
            {
                return GoogleLoginResult.Login;
            }
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, true);
                return GoogleLoginResult.Login;
            }

            user = new ApplicationUser { UserName = email, Email = email };
            var signInResult = await _userManager.AddLoginAsync(user, info);
            if (!signInResult.Succeeded)
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

        public async Task<bool> CheckVerificationTokenAsync(EmailConfirmDto dto)
        {
            ApplicationUser? foundUser = await _userManager.FindByIdAsync(dto.UserId);
            if (foundUser == null)
            {
                return false;
            }

            var result = await _userManager.ConfirmEmailAsync(foundUser, dto.VerificationToken);

            if (!result.Succeeded)
            {
                return false;
            }

            await _signInManager.SignInAsync(foundUser, true);

            return true;
        }

        public async Task EditUserAsync(string userId, UserEditDto dto)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return;
            }
            user.Email = dto.Email.Trim().ToLower();
            if (dto.NewPassword != null && dto.NewPassword != "" && dto.CurrentPassword != null && dto.CurrentPassword != "")
            {
                await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            }

            user.PhoneNumber = dto.Phone?.Trim();
            user.FullName = dto.FullName.Trim();
            await _context.SaveChangesAsync();
        }

        public async Task<FindUserResult> LoginAsync(string email, string password)
        {
            ApplicationUser? foundUser = await _userManager.FindByEmailAsync(email);

            if (foundUser == null)
            {
                return FindUserResult.NotFound;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(foundUser, password, false);

            if (result.IsNotAllowed)
            {
                return FindUserResult.EmailEntered;
            }

            if (!result.Succeeded)
            {
                return FindUserResult.NotFound;
            }

            await _signInManager.SignInAsync(foundUser, true);

            return FindUserResult.EmailConfirmed;
        }

        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<UserEditDto> FindUserByIdAsync(string id)
        {
            UserEditDto? user = await _context.ApplicationUser
                .Where(u => u.Id == id && u.EmailConfirmed)
                .Select(u => new UserEditDto { Email = u.Email!, FullName = u.FullName!, Phone = u.PhoneNumber })
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
                return (null, FindUserResult.NotFound);
            }

            if (foundUser.EmailConfirmed)
            {
                return (foundUser.Id, FindUserResult.EmailConfirmed);
            }
            return (foundUser.Id, FindUserResult.EmailEntered);
        }

        public enum FindUserResult
        {
            EmailEntered, EmailConfirmed, NotFound
        }
    }
}
