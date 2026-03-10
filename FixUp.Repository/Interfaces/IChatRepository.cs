using FixUp.Repository.Models;
using FixUp.Repository.Models;

using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Threading.Tasks;
using YourProjectName.Models;

namespace FixUp.Repository.Interfaces
{
    public interface IChatRepository
    {
        // קטגוריות
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task AddCategoryAsync(Category category);

        // הודעות
        Task<IEnumerable<Message>> GetAllMessagesAsync();
        Task<IEnumerable<Message>> GetMessagesByCategoryAsync(int categoryId);
        Task AddMessageAsync(Message message);
    }
}