using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;

using FixUp.Service.DTOs;
using FixUp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixUp.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IChatRepository _repository;

        public CategoryService(IChatRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            var categories = await _repository.GetAllCategoriesAsync();
            return categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                IconUrl = c.IconUrl
            });
        }

        public async Task AddAsync(CategoryDTO item)
        {
            var category = new Category
            {
                Name = item.Name,
                IconUrl = item.IconUrl
            };
            // שימי לב: הוספתי פונקציית AddCategoryAsync ב-Repository למטה
            await _repository.AddCategoryAsync(category);
        }

        // מימוש פונקציות חובה מה-IService (ניתן להשאיר ללא מימוש כרגע)
        public async Task<CategoryDTO> GetByIdAsync(int id) => throw new NotImplementedException();
        public async Task UpdateAsync(int id, CategoryDTO item) => throw new NotImplementedException();
        public async Task DeleteAsync(int id) => throw new NotImplementedException();
    }
}