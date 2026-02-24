
using FixUp.Repository.Interfaces;

using FixUp.Repository.Models; // כי כאן נמצאים עכשיו User, Client וכו'

using Microsoft.EntityFrameworkCore;

namespace FixUp.Repository.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly IContext _context;
        public ReviewRepository(IContext context) => _context = context;

        public async Task<Review> GetByIdAsync(int id) => await _context.Reviews.FindAsync(id);

        public async Task<IEnumerable<Review>> GetByProfessionalIdAsync(int profId)
        {
            return await _context.Reviews
                .Where(r => r.ProfessionalId == profId)
                .ToListAsync();
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }
    }
}
