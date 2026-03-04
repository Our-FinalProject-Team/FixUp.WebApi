using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace FixUp.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IContext _context;
        public UserRepository(IContext context) => _context = context;

        public async Task<IEnumerable<User>> GetAllUsersAsync() =>
            await _context.Users.ToListAsync();

        public async Task<User> GetUserByIdAsync(int id) =>
            await _context.Users.FindAsync(id);

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}