using FixUp.Service.Dto;
using FixUp.Service.Interfaces;
using FixUp.Service.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _service;
    public ClientsController(IClientService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> GetById(int id)
    {
        var client = await _service.GetByIdAsync(id);
        if (client == null) return NotFound("הלקוח לא נמצא");
        return Ok(client);
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] ClientDto dto, [FromQuery] string password)
    {
        await _service.RegisterClientAsync(dto, password);
        return Ok("הלקוח נרשם בהצלחה");
    }
    // DELETE: api/Clients/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        // 1. בודקים אם הוא קיים (ופעיל) לפני שמנסים למחוק
        var client = await _service.GetByIdAsync(id);
        if (client == null)
        {
            return NotFound("הלקוח לא נמצא או שכבר נמחק בעבר");
        }

        // 2. קוראים לסרוויס שיבצע "מחיקה לוגית" (עדכון הבוליאני)
        await _service.DeleteAsync(id);

        return NoContent(); // מחזירים הצלחה בלי תוכן
    }
    // עדכון פרטי לקוח
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ClientDto dto)
    {
        var existingClient = await _service.GetByIdAsync(id);
        if (existingClient == null)
        {
            return NotFound("הלקוח לא נמצא או שהוא מחוק");
        }

        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    // התחברות לקוח
    [HttpPost("login")]
    public async Task<ActionResult<ClientDto>> Login([FromQuery] string email, [FromQuery] string password)
    {
        var client = await _service.LoginAsync(email, password);

        if (client == null)
        {
            return Unauthorized("אימייל או סיסמה שגויים, או שהלקוח אינו פעיל");
        }

        return Ok(client);
    }
}