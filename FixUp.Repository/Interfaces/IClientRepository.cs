using FixUp.Repository.Models;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAllClientsAsync();
    Task<Client> GetClientByIdAsync(int id);
    Task AddClientAsync(Client client); // נוסף
    Task UpdateClientAsync(Client client);
    Task DeleteClientAsync(int id);     // נוסף
}