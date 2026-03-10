using FixUp.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FixUp.Service.Dto;
namespace FixUp.Service.Interfaces
{
    // הירושה מהממשק הכללי שהעלית
    public interface IMessageService : IService<MessageDTO>
    {
        // כאן אפשר להוסיף פונקציות ספציפיות לצ'אט אם צריך
        Task<IEnumerable<MessageDTO>> GetByCategoryIdAsync(int categoryId);
    }
}