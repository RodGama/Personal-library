
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Modules.Users.Domain;
using Modules.Users.Infrastructure.Database.Repositories.Interfaces;
using System.Text;

namespace Modules.Users.Infrastructure.Database.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }
        public async Task Add(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetById(Guid userId)
        {
            return _context.Users.Where(x => x.Id == userId).FirstOrDefault();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return _context.Users.Where(x => x.Email == email).FirstOrDefault();
        }

        public async Task Update(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
