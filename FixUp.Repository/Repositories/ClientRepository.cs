using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace FixUp.Repository.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IContext _context;
        public ClientRepository(IContext context) => _context = context;

        public async Task<IEnumerable<Client>> GetAllClientsAsync() =>
            await _context.Clients.ToListAsync();

        public async Task<Client> GetClientByIdAsync(int id) =>
            await _context.Clients.FindAsync(id);

        public async Task UpdateClientAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }
    }
}