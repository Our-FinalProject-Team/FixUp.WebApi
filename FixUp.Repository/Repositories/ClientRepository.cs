using FixUp.Repository.Data;
using FixUp.Repository.Interfaces;
using FixUpSolution.Models;
using Microsoft.EntityFrameworkCore;
namespace FixUp.Repository.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly DataContext _context;
        public ClientRepository(DataContext context) => _context = context;

        public async Task<Client> GetByIdAsync(int id) =>
            await _context.Clients.Include(c => c.MyRequests).FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Client>> GetAllAsync() => await _context.Clients.ToListAsync();

        public async Task UpdateAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }
    }
}
