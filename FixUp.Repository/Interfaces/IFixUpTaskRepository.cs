using FixUpSolution.Models;
namespace FixUp.Repository.Interfaces
{
    public interface IFixUpTaskRepository
    {
        Task<IEnumerable<FixUpTask>> GetAllTasksAsync();
        Task<FixUpTask> GetTaskByIdAsync(int id);
        Task AddTaskAsync(FixUpTask task);
        Task UpdateTaskAsync(FixUpTask task);
        Task DeleteTaskAsync(int id);
    }

}
