
using FixUpSolution.Models;

namespace FixUp.Repository.Interfaces
{
        public interface IUserRepository
        {
            // מחזיר את כל המשתמשים
            Task<IEnumerable<User>> GetAllUsersAsync();

            // מוצא משתמש לפי ה-ID שלו
            Task<User> GetUserByIdAsync(int id);

            // מוסיף משתמש חדש
            Task AddUserAsync(User user);

            // מעדכן משתמש קיים
            Task UpdateUserAsync(User user);

            // מוחק משתמש
            Task DeleteUserAsync(int id);
        }
    }

