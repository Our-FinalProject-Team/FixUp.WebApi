using AutoMapper;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Service.Dto;
using FixUp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FixUpTaskService : IFixUpTaskService
{
    private readonly IFixUpTaskRepository _repository;
    private readonly IMapper _mapper;

    public FixUpTaskService(IFixUpTaskRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FixUpTaskDto>> GetAllAsync()
    {
        var tasks = await _repository.GetAllTasksAsync();
        return _mapper.Map<IEnumerable<FixUpTaskDto>>(tasks);
    }

    public async Task<FixUpTaskDto> GetByIdAsync(int id)
    {
        var task = await _repository.GetTaskByIdAsync(id);
        return _mapper.Map<FixUpTaskDto>(task);
    }

    public async Task AddItem(FixUpTaskDto item)
    {
        var model = _mapper.Map<FixUpTask>(item);
        // לוגיקה: אם סוג המשימה הוא SOS, התאריך המתוכנן הוא עכשיו
        if (model.Type == TaskType.SOS) model.ScheduledDate = DateTime.Now;
        await _repository.AddTaskAsync(model);
    }

    public async Task<double> CalculateFinalPrice(int taskId)
    {
        var task = await _repository.GetTaskByIdAsync(taskId);
        // כאן תבוא לוגיקה של מחיר בסיס + תוספת SOS
        return task.PriceEstimate * 1.2;
    }

    public async Task UpdateItem(int id, FixUpTaskDto item) { /* מימוש */ }
    public async Task DeleteItem(int id) { /* מימוש */ }

    public Task AddAsync(FixUpTaskDto item, string password)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(int id, FixUpTaskDto item)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(FixUpTaskDto item)
    {
        throw new NotImplementedException();
    }
}