using Microsoft.AspNetCore.Mvc;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;

namespace FixUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FixUpTasksController : ControllerBase
    {
        private readonly IFixUpTaskRepository _taskRepo;

        public FixUpTasksController(IFixUpTaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FixUpTask>>> GetAll()
        {
            var tasks = await _taskRepo.GetAllTasksAsync(); //
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FixUpTask>> GetById(int id)
        {
            var task = await _taskRepo.GetTaskByIdAsync(id); //
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> Create(FixUpTask task)
        {
            await _taskRepo.AddTaskAsync(task); //
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, FixUpTask task)
        {
            if (id != task.Id) return BadRequest();
            await _taskRepo.UpdateTaskAsync(task); //
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _taskRepo.DeleteTaskAsync(id); //
            return NoContent();
        }
    }
}