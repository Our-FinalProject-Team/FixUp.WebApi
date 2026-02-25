using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace FixUp.Repository.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IContext _context;
        public CategoryRepository(IContext context) => _context = context;

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync() =>
            await _context.Categories.ToListAsync();

        public async Task<Category> GetCategoryByIdAsync(int id) =>
            await _context.Categories.FindAsync(id);

        public async Task AddCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await GetCategoryByIdAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}