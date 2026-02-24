using FixUp.Repository.Models;

namespace FixUp.Repository.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review> GetByIdAsync(int id);
        Task<IEnumerable<Review>> GetByProfessionalIdAsync(int profId);
        Task AddAsync(Review review);
        Task DeleteAsync(int id);
    }
}
