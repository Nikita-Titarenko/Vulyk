using Vulyk.Domain.Models;

namespace Vulyk.Application.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsers(int page, int pageSize);
    }
}