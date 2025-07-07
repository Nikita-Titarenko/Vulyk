using Humanizer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
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

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AddUserResult> AddUserAsync(EmailInputDto dto)
        {
            var result = await FindUserAsync(dto.Email);
            if (result.Item2 != FindUserResult.NotFound)
            {
                return AddUserResult.EmailAlreadyExist;
            }
            Random random = new Random();
            User user = new User
            {
                Email = dto.Email.Trim().ToLower().Trim(),
                VerificationCode = random.Next(1000000).ToString().PadLeft(6, '0'),
            };

            _context.Add(user);
            await SendVerificationCodeAsync(dto);
            await _context.SaveChangesAsync();
            return AddUserResult.Success;
        }

        public enum AddUserResult
        {
            Success, EmailAlreadyExist
        }

        public async Task SendVerificationCodeAsync(EmailInputDto dto)
        {

        }

        public async Task<bool> CheckVerificationCodeAsync(VerificationCodeConfirmDto dto)
        {
            var user = await _context.User.Where(u => u.Email == dto.Email).FirstOrDefaultAsync();
            if (user == null || user.VerificationCode != dto.VerificationCode){
                return false;
            }
            user.RegisterStatus = RegisterStatus.VerificationCodeConfirmed;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> AddNameAndPassword(NameAndPasswordInputDto dto)
        {
            var user = await _context.User.Where(u => u.Email == dto.Email).FirstOrDefaultAsync();
            user.FullName = dto.Name;
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
            user.Phone = dto.Phone.Trim();
            user.FullName = dto.Name.Trim();
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

        public async Task<UserEditDto?> FindUserAsync(int id)
        {
            return await _context.User
                .Where(u => u.Id == id && u.RegisterStatus == RegisterStatus.Registered)
                .Select(u => new UserEditDto { Email = u.Email, Name = u.FullName, Phone = u.Phone})
                .FirstOrDefaultAsync();
        }

        public async Task<string?> GetUserNameAsync(int id)
        {
            return await _context.User.Where(u => u.Id == id && u.RegisterStatus == RegisterStatus.Registered).Select(u => u.FullName).FirstOrDefaultAsync();
        }

        public async Task<(int?, FindUserResult)> FindUserAsync(string email)
        {
            User? foundUser = await _context.User.FirstOrDefaultAsync(u => email.ToLower() == u.Email.ToLower());
            if (foundUser == null)
            {
                return (null, FindUserResult.NotFound);
            }
            FindUserResult findUserResult = 0;
            switch (foundUser.RegisterStatus) { 
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
