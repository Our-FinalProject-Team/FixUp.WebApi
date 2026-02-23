using FixUp.Repository.Data;
using FixUp.Repository.Interfaces;
using FixUpSolution.Models;
using Microsoft.EntityFrameworkCore;
namespace FixUp.Repository.Repositories
{
    public class FixUpTaskRepository : IFixUpTaskRepository
    {
        private readonly DataContext _context;
        public FixUpTaskRepository(DataContext context) => _context = context;

        public async Task<FixUpTask> GetByIdAsync(int id) =>
            await _context.Tasks.Include(t => t.Client).FirstOrDefaultAsync(t => t.Id == id);

        public async Task<IEnumerable<FixUpTask>> GetByClientIdAsync(int clientId) =>
            await _context.Tasks.Where(t => t.ClientId == clientId).ToListAsync();

        public async Task<IEnumerable<FixUpTask>> GetPendingSOSAsync() =>
            await _context.Tasks.Where(t => t.Type == TaskType.SOS && t.Status == TaskStatus.Pending).ToListAsync();

        public async Task AddAsync(FixUpTask task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FixUpTask task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public Task<IEnumerable<FixUpTask>> GetAllTasksAsync()
        {
            throw new NotImplementedException();
        }

        public Task<FixUpTask> GetTaskByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task AddTaskAsync(FixUpTask task)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTaskAsync(FixUpTask task)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTaskAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
