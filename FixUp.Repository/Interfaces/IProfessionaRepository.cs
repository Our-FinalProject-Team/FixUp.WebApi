using FixUp.Repository.Models;

namespace FixUp.Repository.Interfaces
{
    public interface IProfessionalRepository
    {
        Task<IEnumerable<Professional>> GetAllProfessionalsAsync();
        Task<Professional> GetProfessionalByIdAsync(int id);
        Task<bool> ProfessionalExistsAsync(int id);
        Task UpdateProfessionalAsync(Professional professional);
    }
}