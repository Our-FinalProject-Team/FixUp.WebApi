using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace FixUp.Repository.Repositories
{
    public class FixUpTaskRepository : IFixUpTaskRepository
    {
        private readonly IContext _context;

        public FixUpTaskRepository(IContext context)
        {
            _context = context;
        }

        // שליפת כל המשימות
        public async Task<IEnumerable<FixUpTask>> GetAllTasksAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        // שליפת משימה ספציפית לפי ID
        public async Task<FixUpTask> GetTaskByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.Client)
                .Include(t => t.Professional)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // המימוש שביקשת - שליפת משימות לפי לקוח
        public async Task<IEnumerable<FixUpTask>> GetTasksByClientIdAsync(int clientId)
        {
            return await _context.Tasks
                .Where(t => t.ClientId == clientId)
                .ToListAsync();
        }

       

        // הוספת משימה חדשה
        public async Task AddTaskAsync(FixUpTask task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        // עדכון משימה קיימת
        public async Task UpdateTaskAsync(FixUpTask task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        // מחיקת משימה
        public async Task DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public Task<IEnumerable<FixUpTask>> GetPendingSOSTasksAsync()
        {
            throw new NotImplementedException();
        }
    }
}