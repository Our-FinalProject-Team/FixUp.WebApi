using FixUp.Repository.Models;

namespace FixUp.Repository.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<Client> GetClientByIdAsync(int id);
        Task UpdateClientAsync(Client client);
    }
}