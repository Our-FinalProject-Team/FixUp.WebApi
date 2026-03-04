using FixUp.Service.Dto;

namespace FixUp.Service.Interfaces
{
    public interface IClientService : IService<ClientDto>
    {
        Task RegisterClientAsync(ClientDto clientDto, string password);
        // הוספת פונקציית התחברות
        Task<ClientDto> LoginAsync(string email, string password);
    }
}