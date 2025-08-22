using Microsoft.EntityFrameworkCore;
using Vulyk.Application.Repositories;
using Vulyk.Domain.Models;
using Vulyk.Infrastructure.Data;

namespace Vulyk.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<User>> GetUsers(int page, int pageSize)
        {
            return await _context.ApplicationUser
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new
                {
                    RoleId = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Select(ur => ur.RoleId)
                        .FirstOrDefault(),
                    u.FullName,
                    Email = u.Email!,
                    Status = u.EmailConfirmed ? UserStatus.ConfirmedEmail : UserStatus.NotConfirmedEmail,
                })
                .Select(u => new User
                {
                    Email = u.Email,
                    FullName = u.FullName,
                    Status = u.Status,
                    Role = _context.Roles
                        .Where(r => r.Id == u.RoleId).Select(r => r.Name).FirstOrDefault() == "Administrator" ? UserRole.Admin : UserRole.User
                }).ToListAsync();
        }
    }
}
