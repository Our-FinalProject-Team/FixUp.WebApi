
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace FixUp.Repository.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IContext _context;
        public ClientRepository(IContext context) => _context = context;



        public async Task<IEnumerable<Client>> GetAllAsync() => await _context.Clients.ToListAsync();

        public async Task UpdateAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }
        Task<Client> IClientRepository.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

    }
}
