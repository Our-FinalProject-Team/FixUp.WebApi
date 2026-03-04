using FixUp.Service.Dto;
using FixUp.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FixUp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FixUpTasksController : ControllerBase
    {
        private readonly IFixUpTaskService _taskService;

        public FixUpTasksController(IFixUpTaskService taskService)
        {
            _taskService = taskService;
        }

        // GET: api/FixUpTasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FixUpTaskDto>>> Get()
        {
            return Ok(await _taskService.GetAllAsync());
        }

        // GET: api/FixUpTasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FixUpTaskDto>> Get(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        // POST: api/FixUpTasks
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FixUpTaskDto taskDto)
        {
            // שימי לב: כאן משתמשים ב-AddAsync הכללי מה-IService
            await _taskService.AddAsync(taskDto);
            return Ok("המשימה נוצרה בהצלחה");
        }

        // GET: api/FixUpTasks/price/5 (פונקציה ייחודית מ-IFixUpTaskService)
        [HttpGet("price/{id}")]
        public async Task<ActionResult<double>> GetFinalPrice(int id)
        {
            var price = await _taskService.CalculateFinalPrice(id);
            return Ok(price);
        }
    }
}