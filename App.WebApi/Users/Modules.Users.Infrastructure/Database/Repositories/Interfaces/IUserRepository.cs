

using Modules.Users.Domain;

namespace Modules.Users.Infrastructure.Database.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task Add(User user);
       // Task<List<User>> Get(int PageNumber, int pageQuantity);
        Task<User> GetById(Guid userId);
        Task<User> GetUserByEmail(string email);
        Task Update(User user);
    }
}
