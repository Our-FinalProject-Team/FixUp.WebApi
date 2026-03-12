
using FixUp.Service.Dto;
using FixUp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixUp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessionalsController : ControllerBase
    {
        private readonly IProfessionalService _profService;

        public ProfessionalsController(IProfessionalService profService)
        {
            _profService = profService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfessionalDto>>> GetAll() => Ok(await _profService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<ProfessionalDto>> GetById(int id)
        {
            var prof = await _profService.GetByIdAsync(id);
            if (prof == null) return NotFound("איש המקצוע לא נמצא");
            return Ok(prof);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ProfessionalRegisterDto regDto)
        {
            // regDto מכיל עכשיו גם את הפרטים וגם את Password כי הוא יורש
            await _profService.RegisterProfessionalAsync(regDto, regDto.Password);
            return Ok("נרשם בהצלחה");
        }

        [Authorize(Roles = "Professional")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProfessionalDto profDto)
        {
            await _profService.UpdateAsync(id, profDto);
            return NoContent();
        }

        [Authorize(Roles = "Professional")]
        [HttpPut("update-my-profile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] ProfessionalDto profDto)
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized("לא נמצא זיהוי משתמש בטוקן");

            await _profService.UpdateByEmailAsync(email, profDto);
            return NoContent();
        }

        [Authorize(Roles = "Professional")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var prof = await _profService.GetByIdAsync(id);
            if (prof == null) return NotFound();
            await _profService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("login")]
        // שימי לב לשינוי: הוספנו [FromBody] ושינינו ל-UserLoginDto
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] UserLoginDto loginDto)
        {
            // עכשיו אנחנו מעבירים לשירות את הנתונים מתוך האובייקט
            var response = await _profService.LoginAsync(loginDto.Email, loginDto.Password);
            if (response == null) return Unauthorized("אימייל או סיסמה שגויים");
            return Ok(response);
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromQuery] string email, [FromQuery] string newPassword)
        {
            var success = await _profService.UpdatePasswordAsync(email, newPassword);
            if (!success) return NotFound("לא נמצא משתמש פעיל");
            return Ok("הסיסמה עודכנה בהצלחה");
        }
    }
}