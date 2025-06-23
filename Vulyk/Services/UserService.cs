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

        public async Task<int> AddUserAsync(UserRegisterDto dto)
        {
            User user = new User
            {
                Email = dto.Email.Trim().ToLower().Trim(),
                Name = dto.Name.Trim(),
                Login = dto.Login.Trim().ToLower(),
                Password = dto.Password.Trim(),
                Phone = dto.Phone.Trim(),
            };

            _context.Add(user);
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
            user.Name = dto.Name.Trim();
            await _context.SaveChangesAsync();
        }

        public async Task<int?> FindUserAsync(string login, string password)
        {
            User? foundUser = await _context.User.FirstOrDefaultAsync(u => login.ToLower() == u.Login.ToLower() && password == u.Password);
            if (foundUser == null)
            {
                return null;
            }
            return foundUser.Id;
        }

        public async Task<UserEditDto?> FindUserAsync(int id)
        {
            return await _context.User
                .Where(u => u.Id == id)
                .Select(u => new UserEditDto { Email = u.Email, Name = u.Name, Phone = u.Phone})
                .FirstOrDefaultAsync();
        }

        public async Task<string?> GetUserLoginAsync(int id)
        {
            return await _context.User.Where(u => u.Id == id).Select(u => u.Login).FirstOrDefaultAsync();
        }

        public async Task<int?> FindUserAsync(string login, string phone, CreateType createType)
        {
            User? foundUser;
            if (createType.Equals(CreateType.Login))
            {
                foundUser = await _context.User.FirstOrDefaultAsync(u => login == u.Login);
            } else
            {
                foundUser = await _context.User.FirstOrDefaultAsync(u => phone == u.Phone);
            }
            if (foundUser == null)
            {
                return null;
            }
            return foundUser.Id;
        }

        public async Task<Dictionary<string, string>> CheckUniqueColumnsAsync(int? userId, string? login, string? email, string? phone)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            User? existingUser = await _context.User.FirstOrDefaultAsync(u => userId != u.Id && (u.Login == login || u.Email == email || u.Phone == phone));
            if (existingUser != null)
            {
                if (existingUser.Login.Equals(login, StringComparison.CurrentCultureIgnoreCase))
                {
                    errors.Add("Login", "This login have already taken! Choose another");
                }
                if (existingUser.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase))
                {
                    errors.Add("Email", "This email have already taken! Choose another");
                }
                if (existingUser.Phone.Equals(phone, StringComparison.CurrentCultureIgnoreCase))
                {
                    errors.Add("Phone", "This phone have already taken! Choose another");
                }
            }
            return errors;
        }
    }
}
