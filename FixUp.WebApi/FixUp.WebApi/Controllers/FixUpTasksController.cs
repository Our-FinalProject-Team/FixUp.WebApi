using FixUp.Service.Dto;
using FixUp.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FixUpTasksController : ControllerBase
{
    private readonly IFixUpTaskService _taskService;

    public FixUpTasksController(IFixUpTaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FixUpTaskDto>>> GetAll() => Ok(await _taskService.GetAllAsync());

    [HttpPost]
    public async Task<ActionResult> Create(FixUpTaskDto taskDto)
    {
        await _taskService.AddAsync(taskDto);
        return CreatedAtAction(nameof(GetAll), new { id = taskDto.Id }, taskDto);
    }

    [HttpGet("calculate-price/{id}")]
    public async Task<ActionResult<double>> GetPrice(int id)
    {
        var price = await _taskService.CalculateFinalPrice(id);
        return Ok(price);
    }
}