using FixUp.Service.Dto;

namespace FixUp.Service.Interfaces
{
    public interface IProfessionalService : IService<ProfessionalDto>
    {
        Task RegisterProfessionalAsync(ProfessionalDto profDto, string password);
        // הוספת פונקציית התחברות
        Task<AuthResponseDto> LoginAsync(string email, string password);
        Task<bool> UpdatePasswordAsync(string email, string newPassword);
        Task UpdateByEmailAsync(string email, ProfessionalDto item);
    }
}