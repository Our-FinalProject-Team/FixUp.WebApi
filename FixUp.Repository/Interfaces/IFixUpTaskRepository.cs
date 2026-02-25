using FixUp.Repository.Models;

namespace FixUp.Repository.Interfaces
{
    public interface IFixUpTaskRepository
    {
        Task<IEnumerable<FixUpTask>> GetAllTasksAsync();
        Task<FixUpTask> GetTaskByIdAsync(int id);

        
        Task<IEnumerable<FixUpTask>> GetTasksByClientIdAsync(int clientId);
        Task<IEnumerable<FixUpTask>> GetPendingSOSTasksAsync();
        Task AddTaskAsync(FixUpTask task);
        Task UpdateTaskAsync(FixUpTask task);
        Task DeleteTaskAsync(int id);
    }
}