using FixUp.Repository.Interfaces;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Repository.Models;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YourProjectName.Models;

namespace FixUp.Repository.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly IContext _context;

        public ChatRepository(IContext context)
        {
            _context = context;
        }

        // --- לוגיקה של קטגוריות ---

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        // --- לוגיקה של הודעות ---

        public async Task<IEnumerable<Message>> GetAllMessagesAsync()
        {
            return await _context.Messages.ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesByCategoryAsync(int categoryId)
        {
            // חשוב מאוד: הסינון חייב להתאים ל-categoryId ששלחת ב-Swagger
            return await _context.Messages
                .Where(m => m.CategoryId == categoryId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task AddMessageAsync(Message message)
        {
            _context.Messages.Add( message);
            await _context.SaveChangesAsync();
        }
    }
}