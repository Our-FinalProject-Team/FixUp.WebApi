using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public interface IService<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task AddAsync(T item); // הוספה רגילה בלי סיסמה
    Task UpdateAsync(int id, T item);
    Task DeleteAsync(int id);
}