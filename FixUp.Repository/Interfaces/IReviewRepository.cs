using FixUp.Repository.Models;

namespace FixUp.Repository.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review> GetReviewByIdAsync(int id);
        Task<IEnumerable<Review>> GetReviewsByProfessionalIdAsync(int profId);
        Task AddReviewAsync(Review review);
        Task DeleteReviewAsync(int id);
    }
}