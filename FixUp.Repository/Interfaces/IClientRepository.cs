using FixUp.Repository.Models;

namespace FixUp.Repository.Interfaces
{
    public interface IClientRepository
    {
        Task<Client> GetByIdAsync(int id);
        Task<IEnumerable<Client>> GetAllAsync();
        Task UpdateAsync(Client client);
    }
}
