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
        await _service.RegisterClientAsync(dto, password);
        return Ok("הלקוח נרשם בהצלחה");
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

    [Authorize(Roles = "Client")]
    [HttpPut("update-my-profile")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] ClientDto dto)
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email)) return Unauthorized("לא נמצא זיהוי לקוח בטוקן");

        await _service.UpdateByEmailAsync(email, dto);
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromQuery] string email, [FromQuery] string password)
    {
        var response = await _service.LoginAsync(email, password);
        if (response == null) return Unauthorized("אימייל או סיסמה שגויים");
        return Ok(response);
    }
}