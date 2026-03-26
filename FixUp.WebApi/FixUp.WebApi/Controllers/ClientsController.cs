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
    public async Task<IActionResult> Register([FromBody] ClientRegisterDto regDto)
    {
        await _service.RegisterClientAsync(regDto, regDto.Password);
        return Ok("הלקוח נרשם בהצלחה");
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


    [Authorize] // כל משתמש מחובר עם טוקן תקין יכול לגשת
    [HttpGet("me")]
    public async Task<ActionResult<ProfessionalDto>> GetMyProfile()
    {
        // שליפת האימייל מתוך ה-Claims שבטוקן
        var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(id))
            return Unauthorized("לא נמצא זיהוי משתמש בטוקן");

        // שליפת הנתונים מהשירות לפי האימייל
        var prof = await _service.GetByIdAsync(int.Parse(id));

        if (prof == null)
            return NotFound("המשתמש לא נמצא במערכת");

        return Ok(prof);
    }


    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] UserLoginDto loginDto)
    {
        var response = await _service.LoginAsync(loginDto.Email, loginDto.Password);
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