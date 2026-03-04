using FixUp.Service.Dto;

namespace FixUp.Service.Interfaces
{
    public interface IFixUpTaskService : IService<FixUpTaskDto>
    {
        // כאן נשארת רק הפונקציה הייחודית למשימות (ללא SOS)
        Task<double> CalculateFinalPrice(int taskId);
    }
}