using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace FixUp.Repository.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IContext _context;
        public ClientRepository(IContext context) => _context = context;

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            // מחזיר רק לקוחות שהם לא מחוקים
            return await _context.Clients
                                 .Where(c => !c.IsDeleted)
                                 .ToListAsync();
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            // מחזיר לקוח רק אם הוא קיים ואינו מחוק
            return await _context.Clients
                                 .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task UpdateClientAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteClientAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                client.IsDeleted = true; // סימון המחיקה
                await _context.SaveChangesAsync(); // עדכון במסד הנתונים
            }
        }


        public async Task AddClientAsync(Client client)
        {
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
        }
    }
}