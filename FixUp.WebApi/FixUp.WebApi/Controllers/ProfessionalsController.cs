using FixUp.Service.Dto;
using FixUp.Service.Interfaces;
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

        // GET: api/Professionals (מגיע מ-IService)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfessionalDto>>> GetAll()
        {
            var professionals = await _profService.GetAllAsync();
            return Ok(professionals);
        }

        // GET: api/Professionals/5 (מגיע מ-IService)
        [HttpGet("{id}")]
        public async Task<ActionResult<ProfessionalDto>> GetById(int id)
        {
            var prof = await _profService.GetByIdAsync(id);
            if (prof == null) return NotFound("איש המקצוע לא נמצא");
            return Ok(prof);
        }

        // POST: api/Professionals/register (פונקציה ייחודית)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ProfessionalDto profDto, [FromQuery] string password)
        {
            try
            {
                await _profService.RegisterProfessionalAsync(profDto, password);
                return Ok("איש המקצוע נרשם בהצלחה במערכת");
            }
            catch (Exception ex)
            {
                // מחזיר את ההודעה שכתבנו ב-Service (כמו "חסר @")
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Professionals/5 (מגיע מ-IService)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProfessionalDto profDto)
        {
            await _profService.UpdateAsync(id, profDto);
            return NoContent();
        }

        // DELETE: api/Professionals/5 (מגיע מ-IService)
       
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // בדיקה אם קיים לפני מחיקה (אופציונלי)
            var prof = await _profService.GetByIdAsync(id);
            if (prof == null) return NotFound();

            await _profService.DeleteAsync(id);
            return NoContent();
        }
        [HttpPost("login")]
        public async Task<ActionResult<ProfessionalDto>> Login([FromQuery] string email, [FromQuery] string password)
        {
            // הקריאה לסרוויס תבדוק גם אימייל/סיסמה וגם שהמשתמש לא מחוק (IsDeleted)
            var prof = await _profService.LoginAsync(email, password);

            if (prof == null)
            {
                return Unauthorized("אימייל או סיסמה שגויים, או שאיש המקצוע אינו פעיל במערכת");
            }

            return Ok(prof);
        }
    }
}