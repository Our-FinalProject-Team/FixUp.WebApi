using FixUp.Service.Dto;
using FixUp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        try
        {
            await _service.RegisterClientAsync(dto, password);
            return Ok("הלקוח נרשם בהצלחה");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "Client")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ClientDto dto)
    {
        var existingClient = await _service.GetByIdAsync(id);
        if (existingClient == null) return NotFound("הלקוח לא נמצא");
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [Authorize(Roles = "Client")]
    [HttpPut("update-my-profile")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] ClientDto dto)
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email)) return Unauthorized("לא נמצא זיהוי לקוח בטוקן");

        await _service.UpdateByEmailAsync(email, dto);
        return NoContent();
    }

    [Authorize(Roles = "Client")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var client = await _service.GetByIdAsync(id);
        if (client == null) return NotFound("הלקוח לא נמצא");
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromQuery] string email, [FromQuery] string password)
    {
        var response = await _service.LoginAsync(email, password);
        if (response == null) return Unauthorized("אימייל או סיסמה שגויים");
        return Ok(response);
    }

    [HttpPut("reset-password")]
    public async Task<IActionResult> ResetPassword([FromQuery] string email, [FromQuery] string newPassword)
    {
        var success = await _service.UpdatePasswordAsync(email, newPassword);
        if (!success) return NotFound("לא נמצא משתמש פעיל");
        return Ok("הסיסמה עודכנה בהצלחה");
    }
}