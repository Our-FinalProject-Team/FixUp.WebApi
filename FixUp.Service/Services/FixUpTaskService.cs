using AutoMapper;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Service.Dto;
using FixUp.Service.Interfaces;

namespace FixUp.Service.Services
{
    public class FixUpTaskService : IFixUpTaskService
    {
        private readonly IFixUpTaskRepository _repository;
        private readonly IMapper _mapper;

        public FixUpTaskService(IFixUpTaskRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FixUpTaskDto>> GetAllAsync() =>
            _mapper.Map<IEnumerable<FixUpTaskDto>>(await _repository.GetAllTasksAsync());

        public async Task<FixUpTaskDto> GetByIdAsync(int id) =>
            _mapper.Map<FixUpTaskDto>(await _repository.GetTaskByIdAsync(id));

        // מימוש AddAsync מה-IService (במקום AddItem)
        public async Task AddAsync(FixUpTaskDto item)
        {
            var model = _mapper.Map<FixUpTask>(item);
            if (model.ScheduledDate == default) model.ScheduledDate = DateTime.Now;
            await _repository.AddTaskAsync(model);
        }

        public async Task UpdateAsync(int id, FixUpTaskDto item)
        {
            var task = await _repository.GetTaskByIdAsync(id);
            if (task != null)
            {
                _mapper.Map(item, task);
                await _repository.UpdateTaskAsync(task);
            }
        }

        public async Task DeleteAsync(int id) => await _repository.DeleteTaskAsync(id);

        public async Task<double> CalculateFinalPrice(int taskId)
        {
            var task = await _repository.GetTaskByIdAsync(taskId);
            // לוגיקה נקייה: פשוט מחזיר את הערכת המחיר (בלי תוספת SOS)
            return task.PriceEstimate;
        }
    }
}