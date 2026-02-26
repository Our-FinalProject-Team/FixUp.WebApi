using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace FixUp.Repository.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly IContext _context;
        public ReviewRepository(IContext context) => _context = context;

        public async Task<Review> GetReviewByIdAsync(int id) =>
            await _context.Reviews.FindAsync(id);

        public async Task<IEnumerable<Review>> GetReviewsByProfessionalIdAsync(int profId) =>
            await _context.Reviews.Where(r => r.ProfessionalId == profId).ToListAsync();

        public async Task AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await GetReviewByIdAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }
    }
}