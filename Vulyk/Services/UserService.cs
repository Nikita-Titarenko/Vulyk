using System;
using Google.Apis.Auth;
using Humanizer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Security;
using Vulyk.Data;
using Vulyk.Data.Migrations;
using Vulyk.DTOs;
using Vulyk.Models;
using Vulyk.ViewModels;

namespace Vulyk.Services
{
    public class UserService
    {
        private ApplicationDbContext _context;

        private EmailService _emailService;

        public UserService(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<AddUserResult> AddUserAsync(EmailInputDto dto)
        {
            var result = await FindUserAsync(dto.Email);
            if (result.Item2 != FindUserResult.NotFound)
            {
                return AddUserResult.EmailAlreadyExist;
            }
            Random random = new Random();
            string verificationCode = random.Next(1000000).ToString().PadLeft(6, '0');
            User user = new User
            {
                Email = dto.Email.Trim().ToLower().Trim(),
                VerificationCode = verificationCode,
            };

            _context.Add(user);
            await SendVerificationCodeAsync(dto.Email, verificationCode);
            await _context.SaveChangesAsync();
            return AddUserResult.Success;
        }

        public async Task<GoogleSignInResultDto?> GoogleSignIn(string id_token)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { "539615638742-r80f81961bev61udupdefg86dt6rfljp.apps.googleusercontent.com" }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(id_token, settings);

                User? user = await _context.User.Where(u => u.ProviderUserId == payload.Subject).FirstOrDefaultAsync();

                if (user == null)
                {
                    user = await _context.User.Where(u => u.Email == payload.Email).FirstOrDefaultAsync();
                    if (user == null)
                    {
                        user = new User
                        {
                            Email = payload.Email,
                            ProviderUserId = payload.Subject
                        };
                        _context.User.Add(user);
                        await _context.SaveChangesAsync();
                        return new GoogleSignInResultDto { Email = payload.Email, FullName = payload.Name };
                    }

                    user.ProviderUserId = payload.Subject;
                    await _context.SaveChangesAsync();
                }

                if (user.RegisterStatus != RegisterStatus.Registered)
                {
                    return new GoogleSignInResultDto { Email = payload.Email, FullName = payload.Name };
                }

                return new GoogleSignInResultDto { UserId = user.Id };
            }
            catch
            {
                return null;
            }
        }

        public enum AddUserResult
        {
            Success, EmailAlreadyExist
        }

        public async Task SendVerificationCodeAsync(string email, string verificationCode)
        {
            await _emailService.SendEmailAsync(email, "Registration in Vulyk", $"<h1>Confirm your registration in Vulyk</h1><h1>Verification code: {verificationCode}</h1>");
        }

        public async Task<bool> CheckVerificationCodeAsync(VerificationCodeConfirmDto dto)
        {
            var user = await _context.User.Where(u => u.Email == dto.Email).FirstOrDefaultAsync();
            if (user == null || user.VerificationCode != dto.VerificationCode)
            {
                return false;
            }
            user.RegisterStatus = RegisterStatus.VerificationCodeConfirmed;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> AddNameAndPassword(NameAndPasswordInputDto dto)
        {
            var user = await _context.User.Where(u => u.Email == dto.Email).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new InvalidKeyException();
            }
            user.FullName = dto.FullName;
            user.Password = dto.Password;
            user.RegisterStatus = RegisterStatus.Registered;
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task EditUserAsync(int userId, UserEditDto dto)
        {
            User? user = _context.User.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
            {
                return;
            }
            user.Email = dto.Email.Trim().ToLower();
            if (dto.Password != null && dto.Password != "")
            {
                user.Password = dto.Password.Trim().ToLower();
            }

            user.Phone = dto.Phone?.Trim();

            user.FullName = dto.FullName.Trim();
            await _context.SaveChangesAsync();
        }

        public async Task<int?> FindUserAsync(string email, string password)
        {
            User? foundUser = await _context.User.FirstOrDefaultAsync(u => email.ToLower() == u.Email.ToLower() && password == u.Password && u.RegisterStatus == RegisterStatus.Registered);
            if (foundUser == null)
            {
                return null;
            }
            return foundUser.Id;
        }

        public async Task<UserEditDto> FindUserAsync(int id)
        {
            UserEditDto? user = await _context.User
                .Where(u => u.Id == id && u.RegisterStatus == RegisterStatus.Registered)
                .Select(u => new UserEditDto { Email = u.Email, FullName = u.FullName!, Phone = u.Phone })
                .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new InvalidKeyException();
            }

            return user;
        }

        public async Task<string?> GetUserNameAsync(int id)
        {
            return await _context.User.Where(u => u.Id == id && u.RegisterStatus == RegisterStatus.Registered).Select(u => u.FullName).FirstOrDefaultAsync();
        }

        public async Task<(int?, FindUserResult)> FindUserAsync(string email)
        {
            string emailNormalized = email.ToLower();
            User? foundUser = await _context.User.FirstOrDefaultAsync(u => emailNormalized == u.Email.ToLower());
            if (foundUser == null)
            {
                return (null, FindUserResult.NotFound);
            }
            FindUserResult findUserResult = 0;
            switch (foundUser.RegisterStatus)
            {
                case RegisterStatus.Registered:
                    findUserResult = FindUserResult.Registered;
                    break;
                case RegisterStatus.VerificationCodeConfirmed:
                    findUserResult = FindUserResult.VerificationCodeConfirmed;
                    break;
                case RegisterStatus.EmailInputted:
                    findUserResult = FindUserResult.EmailInputted;
                    break;
            }
            return (foundUser.Id, findUserResult);
        }

        public enum FindUserResult
        {
            EmailInputted, VerificationCodeConfirmed, Registered, NotFound
        }
    }
}
