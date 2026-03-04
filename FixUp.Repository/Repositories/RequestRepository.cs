using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace FixUp.Repository.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly IContext _context; // החליפי במידת הצורך לשם של ה-DbContext שלך

        public RequestRepository(IContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Request>> GetAllAsync()
        {
            // מחזיר את כל הבקשות מהטבלה
            return await _context.Requests.ToListAsync();
        }

        public async Task<Request> GetByIdAsync(int id)
        {
            // מוצא בקשה ספציפית לפי ה-ID שלה
            return await _context.Requests.FindAsync(id);
        }

        public async Task<Request> AddAsync(Request request)
        {
            // מוסיף בקשה חדשה לטבלה
            await _context.Requests.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task UpdateAsync(Request request)
        {
            // מעדכן בקשה קיימת
            _context.Requests.Update(request);
            await _context.SaveChangesAsync();
        }
       
        public async Task DeleteAsync(int id)
        {
            // מוחק בקשה לפי ID
            var request = await GetByIdAsync(id);
            if (request != null)
            {
                _context.Requests.Remove(request);
                await _context.SaveChangesAsync();
            }
        }
    }
}