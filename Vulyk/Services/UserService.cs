using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Vulyk.Data;
using Vulyk.Data.Migrations;

namespace Vulyk.Services
{
    public class UserService
    {
        private ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user)
        {
            ConvertToLowerCase(user);
            _context.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task EditUserAsync(User user)
        {
            ConvertToLowerCase(user);
            _context.User.Update(user);
            await _context.SaveChangesAsync();
        }

        private void ConvertToLowerCase(User user)
        {
            user.Login = user.Login.ToLower();
            user.Phone = user.Phone.ToLower();
            user.Name = user.Name.ToLower();
            user.Email = user.Email.ToLower();
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

        public async Task<User?> FindUserAsync(int id)
        {
            return await _context.User.FirstOrDefaultAsync(u => id == u.Id);
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
