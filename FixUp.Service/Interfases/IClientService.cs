using FixUp.Service.Dto;

namespace FixUp.Service.Interfaces
{
    public interface IClientService : IService<ClientDto>
    {
        Task RegisterClientAsync(ClientDto clientDto, string password);
        // הוספת פונקציית התחברות
        Task<AuthResponseDto> LoginAsync(string email, string password);
        Task<bool> UpdatePasswordAsync(string email, string newPassword);
        Task UpdateByEmailAsync(string email, ClientDto item);
    }
}