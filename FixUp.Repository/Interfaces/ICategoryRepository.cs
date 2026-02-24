using FixUp.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixUp.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        
        Task<Category> GetCategoryByIdAsync(int id);

        
        Task AddCategoryAsync(Category category);

        Task UpdateCategoryAsync(Category category);

       
        Task DeleteCategoryAsync(int id);
    }
}
